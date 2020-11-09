INSERT INTO LibraryItem
    (
    "category_id",
    "title",
    "author",
    "pages",
    "run_time_minutes",
    "is_borrowable",
    "borrower",
    "borrow_date"
    ,"type")
VALUES(
        1,
        'En tittel helt awesome',
        'Författarfan',
        123,
        123,
        1,
        null,
        null,
        'Book'
);

SELECT * FROM LibraryItem;