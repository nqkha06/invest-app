import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  ManyToOne,
  JoinColumn,
  Index,
  Unique,
} from 'typeorm';
import { Stock } from './stock.entity';

@Entity('stock_prices_day')
@Unique(['stockId', 'tradingDate'])
@Index(['stockId', 'tradingDate'])
export class StockPriceDay {
  @PrimaryGeneratedColumn('increment', { type: 'bigint' })
  id: number;

  @Column({ type: 'bigint', name: 'stock_id' })
  stockId: number;

  @Column({ type: 'decimal', precision: 18, scale: 4, name: 'open_price' })
  openPrice: number;

  @Column({ type: 'decimal', precision: 18, scale: 4, name: 'high_price' })
  highPrice: number;

  @Column({ type: 'decimal', precision: 18, scale: 4, name: 'low_price' })
  lowPrice: number;

  @Column({ type: 'decimal', precision: 18, scale: 4, name: 'close_price' })
  closePrice: number;

  @Column({ type: 'bigint', default: 0 })
  volume: number;

  @Column({ type: 'date', name: 'trading_date' })
  tradingDate: string;

  // Relations
  @ManyToOne(() => Stock, (stock) => stock.dayPrices, { onDelete: 'CASCADE' })
  @JoinColumn({ name: 'stock_id' })
  stock: Stock;
}
