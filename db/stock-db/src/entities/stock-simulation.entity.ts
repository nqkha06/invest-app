import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  UpdateDateColumn,
  OneToOne,
  JoinColumn,
} from 'typeorm';
import { Stock } from './stock.entity';

export enum AlgorithmType {
  RANDOM_WALK = 'random_walk',
  MEAN_REVERSION = 'mean_reversion',
  MOMENTUM = 'momentum',
  GBM = 'gbm', // Geometric Brownian Motion
}

@Entity('stock_simulations')
export class StockSimulation {
  @PrimaryGeneratedColumn('increment', { type: 'bigint' })
  id: number;

  @Column({ type: 'bigint', name: 'stock_id', unique: true })
  stockId: number;

  @Column({
    type: 'varchar',
    length: 100,
    name: 'algorithm_type',
    default: AlgorithmType.GBM,
  })
  algorithmType: string;

  @Column({
    type: 'decimal',
    precision: 10,
    scale: 6,
    default: 0.02,
  })
  volatility: number;

  @Column({
    type: 'decimal',
    precision: 10,
    scale: 6,
    name: 'trend_factor',
    default: 0.0,
  })
  trendFactor: number;

  @Column({
    type: 'decimal',
    precision: 18,
    scale: 4,
    name: 'min_price',
    nullable: true,
  })
  minPrice: number | null;

  @Column({
    type: 'decimal',
    precision: 18,
    scale: 4,
    name: 'max_price',
    nullable: true,
  })
  maxPrice: number | null;

  @Column({
    type: 'decimal',
    precision: 10,
    scale: 4,
    name: 'update_speed',
    default: 1.0,
    comment: 'Seconds between price updates',
  })
  updateSpeed: number;

  @Column({
    type: 'decimal',
    precision: 5,
    scale: 4,
    name: 'jump_probability',
    default: 0.001,
    comment: 'Probability of a sudden price jump (0-1)',
  })
  jumpProbability: number;

  @UpdateDateColumn({ type: 'datetime', name: 'updated_at' })
  updatedAt: Date;

  // Relations
  @OneToOne(() => Stock, (stock) => stock.simulation, { onDelete: 'CASCADE' })
  @JoinColumn({ name: 'stock_id' })
  stock: Stock;
}
