alter table TBLFaculty_Calaluan
add constraint check_salary
check (Salary < 50000)