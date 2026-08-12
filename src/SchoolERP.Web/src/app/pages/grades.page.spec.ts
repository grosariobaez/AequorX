import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { I18nService } from '../core/i18n.service';
import { GradesPage } from './grades.page';

describe('Grades workflow', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [GradesPage], providers: [provideHttpClient(), provideHttpClientTesting()] }).compileComponents();
  });
  afterEach(() => TestBed.inject(HttpTestingController).verify());

  it('selects a class, saves a Draft, publishes explicitly, and localizes the workflow', async () => {
    const fixture = TestBed.createComponent(GradesPage);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
    http.expectOne('/api/sections').flush([{ id: 'section-1', name: 'A', code: 'A', capacity: 30, academicYearId: 'year-1', academicYearName: '2026-2027', gradeLevelId: 'grade-1', gradeLevelName: 'Primero', campusId: 'campus-1', campusName: 'Principal' }]);
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();

    const section = fixture.nativeElement.querySelector('select') as HTMLSelectElement;
    section.value = 'section-1'; section.dispatchEvent(new Event('change', { bubbles: true }));
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();
    (fixture.nativeElement.querySelector('form button') as HTMLButtonElement).click();
    http.expectOne((request) => request.url === '/api/classes' && request.params.get('sectionId') === 'section-1')
      .flush([{ id: 'class-1', sectionId: 'section-1', subjectId: 'subject-1', name: 'Matemáticas · A', subjectName: 'Matemáticas', subjectCode: 'MAT', isActive: true }]);
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();

    const selectedClass = fixture.nativeElement.querySelectorAll('select')[1] as HTMLSelectElement;
    selectedClass.value = 'class-1'; selectedClass.dispatchEvent(new Event('change', { bubbles: true }));
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();
    (fixture.nativeElement.querySelectorAll('form')[1].querySelector('button') as HTMLButtonElement).click();
    http.expectOne((request) => request.url === '/api/assessments' && request.params.get('classId') === 'class-1')
      .flush([{ id: 'assessment-1', classId: 'class-1', name: 'Quiz', assessmentDate: '2026-09-01', maximumScore: 100, isActive: true }]);
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();

    const assessment = fixture.nativeElement.querySelectorAll('select')[2] as HTMLSelectElement;
    assessment.value = 'assessment-1'; assessment.dispatchEvent(new Event('change', { bubbles: true }));
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();
    const forms = fixture.nativeElement.querySelectorAll('form') as NodeListOf<HTMLFormElement>;
    (forms[3].querySelector('button') as HTMLButtonElement).click();
    http.expectOne('/api/assessments/assessment-1/grades').flush(roster(null, null));
    fixture.detectChanges(); await fixture.whenStable(); fixture.detectChanges();

    const score = fixture.nativeElement.querySelector('tbody input[type="number"]') as HTMLInputElement;
    score.value = '80'; score.dispatchEvent(new Event('input', { bubbles: true }));
    fixture.detectChanges(); await fixture.whenStable();
    (fixture.nativeElement.querySelectorAll('.command-bar button')[0] as HTMLButtonElement).click();
    const draft = http.expectOne('/api/assessments/assessment-1/grades/enrollment-1');
    expect(draft.request.method).toBe('PUT'); expect(draft.request.body).toEqual({ score: 80 });
    draft.flush(roster(80, 'Draft').students[0]);
    http.expectOne('/api/assessments/assessment-1/grades').flush(roster(80, 'Draft'));

    (fixture.nativeElement.querySelectorAll('.command-bar button')[1] as HTMLButtonElement).click();
    http.expectOne('/api/assessments/assessment-1/publish').flush({ published: 1 });
    http.expectOne('/api/assessments/assessment-1/grades').flush(roster(80, 'Published'));
    TestBed.inject(I18nService).setLanguage('en'); fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Grades');
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Published');
  });
});

function roster(score: number | null, status: 'Draft' | 'Published' | null) {
  return { assessmentId: 'assessment-1', assessmentName: 'Quiz', maximumScore: 100, students: [{ gradeId: status ? 'grade-1' : null, enrollmentId: 'enrollment-1', studentNumber: 'S-001', studentName: 'Ana Pérez', score, status }] };
}
