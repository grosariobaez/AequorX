import { Component, inject, signal } from '@angular/core';
import { HealthService } from './core/health.service';

type HealthState = 'checking' | 'healthy' | 'unavailable';

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  private readonly healthService = inject(HealthService);

  protected readonly healthState = signal<HealthState>('checking');

  constructor() {
    this.healthService.checkLiveness().subscribe({
      next: () => this.healthState.set('healthy'),
      error: () => this.healthState.set('unavailable'),
    });
  }
}
