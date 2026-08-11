import { Routes } from '@angular/router';
import { Home } from './home';
import { NotFound } from './not-found';

export const routes: Routes = [
  { path: '', component: Home, pathMatch: 'full' },
  { path: 'not-found', component: NotFound },
  { path: '**', redirectTo: 'not-found' },
];
