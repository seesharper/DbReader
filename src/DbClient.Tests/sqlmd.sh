### sqlmd
# Bash function  for outputting SQLite results in Markdown-friendly table

### Dependency:
# csvlook can be found here: http://csvkit.readthedocs.io/en/540/scripts/csvlook.html

### USAGE
# $ sqlmd "SELECT name, age FROM people;" optional_db_name_argument.sqlite

### OUTPUT

# (stderr) Opening database: optional_db_name_argument.sqlite
# 
# ```sql
# SELECT name, age FROM people;
# ```
#
# | name  |  age  |
# |-------|-------|
# | Alice |  42   |
# | Bob   |   9   |
# {:.table-sql}

# That last line is a Kramdown-style CSS class selector

# Tip: I like piping into OSX's pbcopy for even faster blogging:

# sqlmd "SELECT * FROM mytable;" mydb.sqlite | pbcopy

sqlmd(){
    SQLQUERY="$1"
    # if two arguments, assume second is the database name
    if [ $# -eq 2 ]; then
       THEDBNAME="$2"
       # (stderr) The name of the database being opened, in green-on-black text
       (>&2 printf "\033[1;32m\033[40mOpening database: ${THEDBNAME}\033[m\n\n")
    else
       THEDBNAME=""
    fi
    printf '```sql\n'
    printf "$SQLQUERY"
    printf '\n```\n\n'

# include headers and print results in CSV format
sqlite3 $THEDBNAME <<EOF |
.headers on
.mode csv
.nullvalue NULL
${SQLQUERY}
EOF

csvlook \
    | sed '1d' \
    | sed '$ d' \
    | awk '1; END {print "{:.table-sql}"}'
    # the final line adds a Kramdown-style CSS class to the table, `{:.table-sql}
    # just in case you like styling your data tables
}
