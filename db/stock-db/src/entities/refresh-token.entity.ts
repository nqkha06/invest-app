import {
  Entity,
  PrimaryGeneratedColumn,
  Column,
  CreateDateColumn,
  ManyToOne,
  JoinColumn,
} from 'typeorm';
import { User } from './user.entity';

@Entity('refresh_tokens')
export class RefreshToken {
  @PrimaryGeneratedColumn('increment', { type: 'bigint' })
  id: number;

  @Column({ type: 'bigint', name: 'user_id' })
  userId: number;

  @Column({ type: 'varchar', length: 512, unique: true })
  token: string;

  @Column({ type: 'datetime', name: 'expired_at' })
  expiredAt: Date;

  @Column({ type: 'datetime', name: 'revoked_at', nullable: true })
  revokedAt: Date | null;

  @Column({ type: 'boolean', name: 'is_used', default: false })
  isUsed: boolean;

  @CreateDateColumn({ type: 'datetime', name: 'created_at' })
  createdAt: Date;

  // Relations
  @ManyToOne(() => User, (user) => user.refreshTokens, { onDelete: 'CASCADE' })
  @JoinColumn({ name: 'user_id' })
  user: User;
}
