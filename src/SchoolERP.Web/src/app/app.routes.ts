import { Routes } from '@angular/router';
import { AcademicYearsPage } from './pages/academic-years.page';
import { AttendancePage } from './pages/attendance.page';
import { GradesPage } from './pages/grades.page';
import { EnrollmentsPage } from './pages/enrollments.page';
import { GradeLevelsPage } from './pages/grade-levels.page';
import { PeoplePage } from './pages/people.page';
import { SectionsPage } from './pages/sections.page';
import { StudentsPage } from './pages/students.page';
import { NotFound } from './not-found';

export const routes: Routes = [
  { path: '', redirectTo: 'people', pathMatch: 'full' },
  { path: 'people', component: PeoplePage },
  { path: 'students', component: StudentsPage },
  { path: 'academic-years', component: AcademicYearsPage },
  { path: 'grade-levels', component: GradeLevelsPage },
  { path: 'sections', component: SectionsPage },
  { path: 'enrollments', component: EnrollmentsPage },
  { path: 'attendance', component: AttendancePage },
  { path: 'grades', component: GradesPage },
  { path: 'not-found', component: NotFound },
  { path: '**', redirectTo: 'not-found' },
];
