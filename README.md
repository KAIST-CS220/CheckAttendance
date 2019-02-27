# CheckAttendance

A simple class attendance checker written in F#.

# How Does It Work?

In CS220 @ KAIST, we check the attendance of every student using this
program. It works as follows.

1. First, the course instructor will launch this program at the beginning of a
   class with a certain timeout. The program assumes the existence of a CSV file
   that lists all the student IDs and their names.

2. When the program starts, it shows a random URL that students should connect
   to. Before the timeout expires, the students should connect to the web site,
   and enter their student ID and their last name in the web form.

3. When the timeout expires, the program stores the list of students who have
   successfully checked in. The instructor will then upload the resulting file
   to GitHub (to the records directory).

# 2019 CS220 Attendance Records

Check [this directory](records/).
