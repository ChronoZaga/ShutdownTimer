First thing, any scheduled windows shutdown timers are canceled without user intervention.  Just in case we need an "oh crap" fix at a moments notice.

Then the user is asked how many hours they would like until shutdown, with "0" being no shutdown.  So if a user enters 0 or nothing, no shutdown is scheduled.

After entering a number a windows shutdown is scheduled in that amount of time, and the user is asked if that is correct.  "Y" exits with a shutdown timer set, and "N" cancels any shutdown timer and restarts the program from the top.
