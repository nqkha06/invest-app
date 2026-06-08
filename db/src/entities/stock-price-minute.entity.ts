import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  ManyToOne,
  JoinColumn,
  Index,
} from 'typeorm';
import { Stock } from './stock.entity';

@Entity('stock_prices_minute')
@Index(['stockId', 'recordedAt'])
export class StockPriceMinute {
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

  @Column({ type: 'datetime', name: 'recorded_at' })
  recordedAt: Date;

  // Relations
  @ManyToOne(() => Stock, (stock) => stock.minutePrices, {
    onDelete: 'CASCADE',
  })
  @JoinColumn({ name: 'stock_id' })
  stock: Stock;
}
