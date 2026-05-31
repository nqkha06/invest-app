import { DataSource, DataSourceOptions } from 'typeorm';
import * as dotenv from 'dotenv';

import {
  User,
  RefreshToken,
  Stock,
  UserWatchlist,
  StockSimulation,
  StockPriceMinute,
  StockPriceDay,
} from './entities';

dotenv.config();

export const dataSourceOptions: DataSourceOptions = {
  type: 'mysql',          // change to 'postgres' if using PostgreSQL
  host: process.env.DB_HOST ?? 'localhost',
  port: Number(process.env.DB_PORT) || 3306,
  username: process.env.DB_USERNAME ?? 'root',
  password: process.env.DB_PASSWORD ?? '',
  database: process.env.DB_NAME ?? 'stock_db',
  synchronize: false,     // always false in production — use migrations
  logging: process.env.NODE_ENV === 'development',
  entities: [
    User,
    RefreshToken,
    Stock,
    UserWatchlist,
    StockSimulation,
    StockPriceMinute,
    StockPriceDay,
  ],
  migrations: [__dirname + '/migrations/*.{ts,js}'],
  migrationsTableName: 'typeorm_migrations',
};

// CLI data source (used by `typeorm migration:run`)
const AppDataSource = new DataSource(dataSourceOptions);
export default AppDataSource;
