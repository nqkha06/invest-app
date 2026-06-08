import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  CreateDateColumn,
  OneToMany,
  OneToOne,
} from 'typeorm';
import { UserWatchlist } from './user-watchlist.entity';
import { StockSimulation } from './stock-simulation.entity';
import { StockPriceMinute } from './stock-price-minute.entity';
import { StockPriceDay } from './stock-price-day.entity';

@Entity('stocks')
export class Stock {
  @PrimaryGeneratedColumn('increment', { type: 'bigint' })
  id: number;

  @Column({ type: 'varchar', length: 20, unique: true })
  symbol: string;

  @Column({ type: 'varchar', length: 255, name: 'company_name' })
  companyName: string;

  @Column({ type: 'varchar', length: 100, nullable: true })
  sector: string | null;

  @Column({
    type: 'decimal',
    precision: 18,
    scale: 4,
    name: 'current_price',
    nullable: true,
  })
  currentPrice: number | null;

  @Column({ type: 'boolean', name: 'is_active', default: true })
  isActive: boolean;

  @CreateDateColumn({ type: 'datetime', name: 'created_at' })
  createdAt: Date;

  // Relations
  @OneToMany(() => UserWatchlist, (watchlist) => watchlist.stock)
  watchlists: UserWatchlist[];

  @OneToOne(() => StockSimulation, (simulation) => simulation.stock)
  simulation: StockSimulation;

  @OneToMany(() => StockPriceMinute, (price) => price.stock)
  minutePrices: StockPriceMinute[];

  @OneToMany(() => StockPriceDay, (price) => price.stock)
  dayPrices: StockPriceDay[];
}
