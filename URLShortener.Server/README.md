`CREATE USER urlshortener;`
`ALTER USER urlshortener CREATEDB;`
`ALTER SCHEMA public OWNER TO urlshortener;`
- On Windows 
- `CREATE DATABASE "urlshortener" WITH OWNER "urlshortener" ENCODING 'UTF8' LC_COLLATE = 'en_US.UTF-8' LC_CTYPE = 'en_US.UTF-8';`