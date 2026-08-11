export interface Identifier {
  id: string;
}

export interface Person {
  id: string;
  firstName: string;
  lastName: string;
  email: string | null;
  phone: string | null;
  isActive: boolean;
}

export interface Student {
  personId: string;
  studentNumber: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
}

export interface AcademicYear {
  id: string;
  name: string;
  startDate: string;
  endDate: string;
  status: string;
}

export interface GradeLevel {
  id: string;
  name: string;
  code: string;
  sortOrder: number;
  isActive: boolean;
}

export interface Campus {
  id: string;
  name: string;
  code: string;
  isActive: boolean;
}

export interface Section {
  id: string;
  name: string;
  code: string;
  capacity: number | null;
  academicYearId: string;
  academicYearName: string;
  gradeLevelId: string;
  gradeLevelName: string;
  campusId: string;
  campusName: string;
}

export interface Enrollment {
  id: string;
  studentPersonId: string;
  studentNumber: string;
  studentName: string;
  academicYearId: string;
  academicYearName: string;
  sectionId: string;
  sectionName: string;
  status: string;
  enrollmentDate: string;
}
