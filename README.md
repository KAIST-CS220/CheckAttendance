# CheckAttendance

A simple class attendance checker written in F#.

# How Does It Work?

In CS220 @ KAIST, we check the attendance of every student using this
program. It works as follows!

1. First, the course instructor will launch this program at the beginning of a
   class. The program assumes the existence of a CSV file that lists all the
   student IDs and their names.

2. When the program starts, it shows a random URL that students should connect
   to. The program has a command line prompt showing how many students are
   checked in so far.

3. The instructor can run the `stop` command whenever appropriate, but if there
   are some students having trouble connecting to the system, they can be
   manually checked in with the `add` command.

4. When everything is done, the instructor will run the `quit` command to exit
   the program. It will store to a file the list of students who have
   successfully checked in. The instructor will then upload the resulting file
   to GitHub (to the records directory).

# 2019 CS220 Attendance Records

Check [this directory](records/).
