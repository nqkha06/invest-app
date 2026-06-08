import { MigrationInterface, QueryRunner, Table, TableForeignKey, TableIndex } from 'typeorm';

export class CreateStockDatabase1700000000000 implements MigrationInterface {
  name = 'CreateStockDatabase1700000000000';

  public async up(queryRunner: QueryRunner): Promise<void> {
    // ─────────────────────────────────────────
    // 1. USERS
    // ─────────────────────────────────────────
    await queryRunner.createTable(
      new Table({
        name: 'users',
        columns: [
          {
            name: 'id',
            type: 'bigint',
            isPrimary: true,
            isGenerated: true,
            generationStrategy: 'increment',
          },
          {
            name: 'username',
            type: 'varchar',
            length: '100',
            isUnique: true,
            isNullable: false,
          },
          {
            name: 'email',
            type: 'varchar',
            length: '255',
            isUnique: true,
            isNullable: false,
          },
          {
            name: 'password_hash',
            type: 'varchar',
            length: '255',
            isNullable: false,
          },
          {
            name: 'role',
            type: 'varchar',
            length: '50',
            default: "'user'",
            isNullable: false,
          },
          {
            name: 'is_active',
            type: 'boolean',
            default: true,
            isNullable: false,
          },
          {
            name: 'created_at',
            type: 'datetime',
            default: 'CURRENT_TIMESTAMP',
            isNullable: false,
          },
          {
            name: 'updated_at',
            type: 'datetime',
            default: 'CURRENT_TIMESTAMP',
            onUpdate: 'CURRENT_TIMESTAMP',
            isNullable: false,
          },
        ],
      }),
      true,
    );

    // ─────────────────────────────────────────
    // 2. REFRESH_TOKENS
    // ─────────────────────────────────────────
    await queryRunner.createTable(
      new Table({
        name: 'refresh_tokens',
        columns: [
          {
            name: 'id',
            type: 'bigint',
            isPrimary: true,
            isGenerated: true,
            generationStrategy: 'increment',
          },
          {
            name: 'user_id',
            type: 'bigint',
            isNullable: false,
          },
          {
            name: 'token',
            type: 'varchar',
            length: '512',
            isUnique: true,
            isNullable: false,
          },
          {
            name: 'expired_at',
            type: 'datetime',
            isNullable: false,
          },
          {
            name: 'revoked_at',
            type: 'datetime',
            isNullable: true,
          },
          {
            name: 'is_used',
            type: 'boolean',
            default: false,
            isNullable: false,
          },
          {
            name: 'created_at',
            type: 'datetime',
            default: 'CURRENT_TIMESTAMP',
            isNullable: false,
          },
        ],
      }),
      true,
    );

    await queryRunner.createForeignKey(
      'refresh_tokens',
      new TableForeignKey({
        name: 'FK_refresh_tokens_user_id',
        columnNames: ['user_id'],
        referencedTableName: 'users',
        referencedColumnNames: ['id'],
        onDelete: 'CASCADE',
        onUpdate: 'NO ACTION',
      }),
    );

    await queryRunner.createIndex(
      'refresh_tokens',
      new TableIndex({
        name: 'IDX_refresh_tokens_user_id',
        columnNames: ['user_id'],
      }),
    );

    // ─────────────────────────────────────────
    // 3. STOCKS
    // ─────────────────────────────────────────
    await queryRunner.createTable(
      new Table({
        name: 'stocks',
        columns: [
          {
            name: 'id',
            type: 'bigint',
            isPrimary: true,
            isGenerated: true,
            generationStrategy: 'increment',
          },
          {
            name: 'symbol',
            type: 'varchar',
            length: '20',
            isUnique: true,
            isNullable: false,
          },
          {
            name: 'company_name',
            type: 'varchar',
            length: '255',
            isNullable: false,
          },
          {
            name: 'sector',
            type: 'varchar',
            length: '100',
            isNullable: true,
          },
          {
            name: 'current_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: true,
          },
          {
            name: 'is_active',
            type: 'boolean',
            default: true,
            isNullable: false,
          },
          {
            name: 'created_at',
            type: 'datetime',
            default: 'CURRENT_TIMESTAMP',
            isNullable: false,
          },
        ],
      }),
      true,
    );

    // ─────────────────────────────────────────
    // 4. USER_WATCHLISTS
    // ─────────────────────────────────────────
    await queryRunner.createTable(
      new Table({
        name: 'user_watchlists',
        columns: [
          {
            name: 'id',
            type: 'bigint',
            isPrimary: true,
            isGenerated: true,
            generationStrategy: 'increment',
          },
          {
            name: 'user_id',
            type: 'bigint',
            isNullable: false,
          },
          {
            name: 'stock_id',
            type: 'bigint',
            isNullable: false,
          },
          {
            name: 'created_at',
            type: 'datetime',
            default: 'CURRENT_TIMESTAMP',
            isNullable: false,
          },
        ],
        uniques: [
          {
            name: 'UQ_user_watchlists_user_stock',
            columnNames: ['user_id', 'stock_id'],
          },
        ],
      }),
      true,
    );

    await queryRunner.createForeignKey(
      'user_watchlists',
      new TableForeignKey({
        name: 'FK_user_watchlists_user_id',
        columnNames: ['user_id'],
        referencedTableName: 'users',
        referencedColumnNames: ['id'],
        onDelete: 'CASCADE',
        onUpdate: 'NO ACTION',
      }),
    );

    await queryRunner.createForeignKey(
      'user_watchlists',
      new TableForeignKey({
        name: 'FK_user_watchlists_stock_id',
        columnNames: ['stock_id'],
        referencedTableName: 'stocks',
        referencedColumnNames: ['id'],
        onDelete: 'CASCADE',
        onUpdate: 'NO ACTION',
      }),
    );

    await queryRunner.createIndex(
      'user_watchlists',
      new TableIndex({
        name: 'IDX_user_watchlists_user_id',
        columnNames: ['user_id'],
      }),
    );

    await queryRunner.createIndex(
      'user_watchlists',
      new TableIndex({
        name: 'IDX_user_watchlists_stock_id',
        columnNames: ['stock_id'],
      }),
    );

    // ─────────────────────────────────────────
    // 5. STOCK_SIMULATIONS
    // ─────────────────────────────────────────
    await queryRunner.createTable(
      new Table({
        name: 'stock_simulations',
        columns: [
          {
            name: 'id',
            type: 'bigint',
            isPrimary: true,
            isGenerated: true,
            generationStrategy: 'increment',
          },
          {
            name: 'stock_id',
            type: 'bigint',
            isUnique: true,
            isNullable: false,
          },
          {
            name: 'algorithm_type',
            type: 'varchar',
            length: '100',
            default: "'gbm'",
            isNullable: false,
          },
          {
            name: 'volatility',
            type: 'decimal',
            precision: 10,
            scale: 6,
            default: 0.02,
            isNullable: false,
          },
          {
            name: 'trend_factor',
            type: 'decimal',
            precision: 10,
            scale: 6,
            default: 0.0,
            isNullable: false,
          },
          {
            name: 'min_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: true,
          },
          {
            name: 'max_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: true,
          },
          {
            name: 'update_speed',
            type: 'decimal',
            precision: 10,
            scale: 4,
            default: 1.0,
            comment: 'Seconds between price updates',
            isNullable: false,
          },
          {
            name: 'jump_probability',
            type: 'decimal',
            precision: 5,
            scale: 4,
            default: 0.001,
            comment: 'Probability of sudden price jump (0-1)',
            isNullable: false,
          },
          {
            name: 'updated_at',
            type: 'datetime',
            default: 'CURRENT_TIMESTAMP',
            onUpdate: 'CURRENT_TIMESTAMP',
            isNullable: false,
          },
        ],
      }),
      true,
    );

    await queryRunner.createForeignKey(
      'stock_simulations',
      new TableForeignKey({
        name: 'FK_stock_simulations_stock_id',
        columnNames: ['stock_id'],
        referencedTableName: 'stocks',
        referencedColumnNames: ['id'],
        onDelete: 'CASCADE',
        onUpdate: 'NO ACTION',
      }),
    );

    // ─────────────────────────────────────────
    // 6. STOCK_PRICES_MINUTE
    // ─────────────────────────────────────────
    await queryRunner.createTable(
      new Table({
        name: 'stock_prices_minute',
        columns: [
          {
            name: 'id',
            type: 'bigint',
            isPrimary: true,
            isGenerated: true,
            generationStrategy: 'increment',
          },
          {
            name: 'stock_id',
            type: 'bigint',
            isNullable: false,
          },
          {
            name: 'open_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'high_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'low_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'close_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'volume',
            type: 'bigint',
            default: 0,
            isNullable: false,
          },
          {
            name: 'recorded_at',
            type: 'datetime',
            isNullable: false,
          },
        ],
      }),
      true,
    );

    await queryRunner.createForeignKey(
      'stock_prices_minute',
      new TableForeignKey({
        name: 'FK_stock_prices_minute_stock_id',
        columnNames: ['stock_id'],
        referencedTableName: 'stocks',
        referencedColumnNames: ['id'],
        onDelete: 'CASCADE',
        onUpdate: 'NO ACTION',
      }),
    );

    await queryRunner.createIndex(
      'stock_prices_minute',
      new TableIndex({
        name: 'IDX_stock_prices_minute_stock_recorded',
        columnNames: ['stock_id', 'recorded_at'],
      }),
    );

    // ─────────────────────────────────────────
    // 7. STOCK_PRICES_DAY
    // ─────────────────────────────────────────
    await queryRunner.createTable(
      new Table({
        name: 'stock_prices_day',
        columns: [
          {
            name: 'id',
            type: 'bigint',
            isPrimary: true,
            isGenerated: true,
            generationStrategy: 'increment',
          },
          {
            name: 'stock_id',
            type: 'bigint',
            isNullable: false,
          },
          {
            name: 'open_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'high_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'low_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'close_price',
            type: 'decimal',
            precision: 18,
            scale: 4,
            isNullable: false,
          },
          {
            name: 'volume',
            type: 'bigint',
            default: 0,
            isNullable: false,
          },
          {
            name: 'trading_date',
            type: 'date',
            isNullable: false,
          },
        ],
        uniques: [
          {
            name: 'UQ_stock_prices_day_stock_date',
            columnNames: ['stock_id', 'trading_date'],
          },
        ],
      }),
      true,
    );

    await queryRunner.createForeignKey(
      'stock_prices_day',
      new TableForeignKey({
        name: 'FK_stock_prices_day_stock_id',
        columnNames: ['stock_id'],
        referencedTableName: 'stocks',
        referencedColumnNames: ['id'],
        onDelete: 'CASCADE',
        onUpdate: 'NO ACTION',
      }),
    );

    await queryRunner.createIndex(
      'stock_prices_day',
      new TableIndex({
        name: 'IDX_stock_prices_day_stock_date',
        columnNames: ['stock_id', 'trading_date'],
      }),
    );
  }

  // ─────────────────────────────────────────
  // DOWN — rollback in reverse order
  // ─────────────────────────────────────────
  public async down(queryRunner: QueryRunner): Promise<void> {
    await queryRunner.dropTable('stock_prices_day', true, true, true);
    await queryRunner.dropTable('stock_prices_minute', true, true, true);
    await queryRunner.dropTable('stock_simulations', true, true, true);
    await queryRunner.dropTable('user_watchlists', true, true, true);
    await queryRunner.dropTable('stocks', true, true, true);
    await queryRunner.dropTable('refresh_tokens', true, true, true);
    await queryRunner.dropTable('users', true, true, true);
  }
}
