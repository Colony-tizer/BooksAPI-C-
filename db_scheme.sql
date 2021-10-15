/*
	Books: id, name, desc
	BooksAuth: id, bookId, authId
	Authors: id, name, surname, bio
*/
DROP VIEW IF EXISTS BOOKS_AUTHORS_VIEW;
DROP VIEW IF EXISTS AUTHORS_BOOKS_COUNT_VIEW;
DROP TABLE IF EXISTS books_authors;
DROP TABLE IF EXISTS books;
DROP TABLE IF EXISTS authors;

CREATE TABLE books(
	book_id int GENERATED ALWAYS AS IDENTITY,
	b_name varchar(200) NOT NULL,
	b_releasedate date NOT NULL,
	b_desc text,
	is_hidden bool DEFAULT false,
	PRIMARY KEY(book_id)
);

INSERT INTO books(b_name, b_releasedate)
VALUES('Book1', '2010-01-01'),
      ('Book2: The rise of the Books', '2011-12-20'),
      ('Other Book: Long names are not necessary bad thing, but it makes mermerizing harder than it should be', '1998-06-14'),
      ('The Ancient Book From The Cave', '530-01-01');
	  
CREATE TABLE authors(
	auth_id int GENERATED ALWAYS AS IDENTITY,
	a_name varchar(200) NOT NULL,
	a_surname varchar(200),
	a_sex varchar(50),
	a_birthdate date NOT NULL,
	a_deathdate date,
	a_bio text,
	is_hidden bool DEFAULT false,
	PRIMARY KEY(auth_id)
);

INSERT INTO authors(a_name, a_surname, a_sex, a_birthdate, a_deathdate, a_bio)
VALUES('Paul', 'Exampleoux', 'Male', '1950-12-31', '2000-09-06', NULL),
      ('Maximus', 'Paynimus', 'Male', '330-07-14', '364-05-06', 'The ancient mysterious philosopher from the unknown country'),
      ('Jack', 'Paullack', 'Male', '1970-05-11', NULL, NULL),
	  ('Pitt', 'Oreburg', 'Undefined space octopus', '1968-10-17', NULL, NULL),
	  ('Jayne', 'Kottovich', 'Female', '1980-01-20', NULL, 'The famous female writer'),
	  ('U', null, null, '1980-01-20', NULL, null);

CREATE TABLE books_authors(
	bookauth_id int GENERATED ALWAYS AS IDENTITY,
	book_id int NOT NULL,
	auth_id int NOT NULL,
	CONSTRAINT fk_book
		FOREIGN KEY(book_id) 
		REFERENCES books(book_id),
	CONSTRAINT fk_auth
		FOREIGN KEY(auth_id) 
		REFERENCES authors(auth_id)
);
INSERT INTO public.books_authors(book_id, auth_id)
	VALUES (1, 1),
		   (1, 3),
		   (2, 1),
		   (2, 3),
		   (2, 4),	
		   (3, 5),	
		   (3, 4),	
		   (4, 2);

CREATE OR REPLACE RULE deleteBook AS ON DELETE TO public.books
	DO INSTEAD 
	UPDATE public.books 
	SET is_hidden = true
	WHERE book_id = OLD.book_id;


CREATE OR REPLACE RULE deleteAuthor AS ON DELETE TO public.authors
	DO INSTEAD 
	UPDATE public.authors 
	SET is_hidden = true
	WHERE auth_id = OLD.auth_id;



CREATE OR REPLACE VIEW BOOKS_AUTHORS_VIEW AS
select public.books.book_id, public.books.b_name, array_to_string(array_agg(public.authors.a_surname || ' ' || public.authors.a_name), ', ') AS Authors
from public.books, public.authors, public.books_authors 
where public.books_authors.auth_id = public.authors.auth_id and public.books_authors.book_id = public.books.book_id and authors.is_hidden = false and books.is_hidden = false
group by public.books.book_id, public.books.b_name
order by book_id desc;

CREATE OR REPLACE VIEW AUTHORS_BOOKS_COUNT_VIEW AS
SELECT authors.auth_id, authors.a_name, authors.a_surname, coalesce(books_count, 0) as books_count from authors
LEFT OUTER JOIN (
	SELECT authors.auth_id, COUNT(books_authors.book_id) as books_count from books_authors, authors, books
	where books_authors.auth_id = authors.auth_id and books_authors.book_id = books.book_id and authors.is_hidden = false and books.is_hidden = false
	group by authors.auth_id,authors.a_name, authors.a_surname
) a on a.auth_id = authors.auth_id;

