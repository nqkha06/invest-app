using Microsoft.EntityFrameworkCore;
using StockExchange.Data.Context;
using StockExchange.Data.Repositories.Interfaces;
using StockExchange.Shared.DTOs;
using StockExchange.Shared.Models;

namespace StockExchange.Server.Services;

public class StockSimulationService
{
    private static readonly HashSet<string> AllowedAlgorithms = new(StringComparer.OrdinalIgnoreCase)
    {
        "RandomWalk",
        "MeanReversion",
        "TrendFollowing"
    };

    private readonly StockExchangeDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public StockSimulationService(StockExchangeDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<StockSimulationConfigDto>> GetAllAsync(
        long adminUserId,
        CancellationToken cancellationToken = default)
    {
        await RequireAdminAsync(adminUserId, cancellationToken);
        var simulations = await _unitOfWork.StockSimulations.GetAllAsync(cancellationToken);
        return simulations.Select(ToDto).ToList();
    }

    public async Task<StockSimulationConfigDto> UpdateAsync(
        long adminUserId,
        StockSimulationUpdateDto request,
        CancellationToken cancellationToken = default)
    {
        await RequireAdminAsync(adminUserId, cancellationToken);
        var values = Validate(request);
        var simulation = await _unitOfWork.StockSimulations.GetByIdAsync(values.Id, cancellationToken)
            ?? throw new InvalidOperationException("Simulation config was not found.");

        simulation.AlgorithmType = values.AlgorithmType;
        simulation.Volatility = values.Volatility;
        simulation.TrendFactor = values.TrendFactor;
        simulation.MinPrice = values.MinPrice;
        simulation.MaxPrice = values.MaxPrice;
        simulation.UpdateSpeed = values.UpdateSpeed;
        simulation.JumpProbability = values.JumpProbability;
        _unitOfWork.StockSimulations.Update(simulation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshed = await _unitOfWork.StockSimulations.GetByStockIdAsync(simulation.StockId, cancellationToken)
            ?? simulation;
        return ToDto(refreshed);
    }

    private async Task RequireAdminAsync(long userId, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("User account was not found.");
        if (!user.IsActive || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Administrator permission is required.");
        }
    }

    private static StockSimulationUpdateDto Validate(StockSimulationUpdateDto? request)
    {
        if (request is null)
        {
            throw new InvalidOperationException("Simulation config payload is required.");
        }

        var algorithm = request.AlgorithmType.Trim();
        if (!AllowedAlgorithms.Contains(algorithm))
        {
            throw new InvalidOperationException("Simulation algorithm is invalid.");
        }
        if (request.MinPrice >= request.MaxPrice)
        {
            throw new InvalidOperationException("Minimum price must be lower than maximum price.");
        }
        if (request.UpdateSpeed <= 0)
        {
            throw new InvalidOperationException("Update speed must be greater than zero.");
        }
        if (request.Volatility < 0 || request.Volatility > 1)
        {
            throw new InvalidOperationException("Volatility must be between 0 and 1.");
        }
        if (request.JumpProbability < 0 || request.JumpProbability > 1)
        {
            throw new InvalidOperationException("Jump probability must be between 0 and 1.");
        }
        if (request.MinPrice < 0 || request.MaxPrice < 0)
        {
            throw new InvalidOperationException("Price limits cannot be negative.");
        }

        return new StockSimulationUpdateDto
        {
            Id = request.Id,
            AlgorithmType = AllowedAlgorithms.First(item => string.Equals(item, algorithm, StringComparison.OrdinalIgnoreCase)),
            Volatility = request.Volatility,
            TrendFactor = request.TrendFactor,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            UpdateSpeed = request.UpdateSpeed,
            JumpProbability = request.JumpProbability
        };
    }

    private static StockSimulationConfigDto ToDto(StockSimulation simulation)
    {
        return new StockSimulationConfigDto
        {
            Id = simulation.Id,
            StockId = simulation.StockId,
            Symbol = simulation.Stock?.Symbol ?? string.Empty,
            CompanyName = simulation.Stock?.CompanyName ?? string.Empty,
            AlgorithmType = simulation.AlgorithmType,
            Volatility = simulation.Volatility,
            TrendFactor = simulation.TrendFactor,
            MinPrice = simulation.MinPrice,
            MaxPrice = simulation.MaxPrice,
            UpdateSpeed = simulation.UpdateSpeed,
            JumpProbability = simulation.JumpProbability,
            UpdatedAt = simulation.UpdatedAt
        };
    }
}
