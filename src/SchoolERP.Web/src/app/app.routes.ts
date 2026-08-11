import { Routes } from '@angular/router';
import { NotFound } from './not-found';

export const routes: Routes = [
  { path: 'not-found', component: NotFound },
  { path: '**', redirectTo: 'not-found' },
];
