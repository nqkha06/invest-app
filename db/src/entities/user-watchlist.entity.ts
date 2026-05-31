import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  CreateDateColumn,
  ManyToOne,
  JoinColumn,
  Unique,
} from 'typeorm';
import { User } from './user.entity';
import { Stock } from './stock.entity';

@Entity('user_watchlists')
@Unique(['userId', 'stockId'])
export class UserWatchlist {
  @PrimaryGeneratedColumn('increment', { type: 'bigint' })
  id: number;

  @Column({ type: 'bigint', name: 'user_id' })
  userId: number;

  @Column({ type: 'bigint', name: 'stock_id' })
  stockId: number;

  @CreateDateColumn({ type: 'datetime', name: 'created_at' })
  createdAt: Date;

  // Relations
  @ManyToOne(() => User, (user) => user.watchlists, { onDelete: 'CASCADE' })
  @JoinColumn({ name: 'user_id' })
  user: User;

  @ManyToOne(() => Stock, (stock) => stock.watchlists, { onDelete: 'CASCADE' })
  @JoinColumn({ name: 'stock_id' })
  stock: Stock;
}
