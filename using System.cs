using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LibraryManagementSystemUI
{
    public partial class MainForm : Form
    {
        private Library library;

        public MainForm()
        {
            InitializeComponent();
            library = new Library();
            library.LoadBooksFromFile("books.txt");
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text;
            string author = txtAuthor.Text;
            int year;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || !int.TryParse(txtYear.Text, out year))
            {
                MessageBox.Show("Please enter valid book details.");
                return;
            }

            library.AddBook(new Book(title, author, year));
            txtTitle.Clear();
            txtAuthor.Clear();
            txtYear.Clear();
            MessageBox.Show("Book added successfully!");
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            listBoxBooks.Items.Clear();
            foreach (var book in library.GetBooks())
            {
                listBoxBooks.Items.Add(book);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listBoxBooks.SelectedItem != null)
            {
                string title = listBoxBooks.SelectedItem.ToString().Split(" by ")[0];
                library.DeleteBook(title);
                MessageBox.Show("Book deleted successfully!");
                btnView_Click(sender, e); // Refresh the list
            }
            else
            {
                MessageBox.Show("Please select a book to delete.");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            library.SaveBooksToFile("books.txt");
        }
    }

    public class Book
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }

        public Book(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Year = year;
        }

        public override string ToString()
        {
            return $"{Title} by {Author} ({Year})";
        }
    }

    public class Library
    {
        private List<Book> books;

        public Library()
        {
            books = new List<Book>();
        }

        public void AddBook(Book book)
        {
            books.Add(book);
        }

        public void DeleteBook(string title)
        {
            var bookToDelete = books.FirstOrDefault(b => b.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (bookToDelete != null)
            {
                books.Remove(bookToDelete);
            }
        }

        public List<Book> GetBooks()
        {
            return books;
        }

        public void LoadBooksFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3 &&
                        int.TryParse(parts[2], out int year))
                    {
                        books.Add(new Book(parts[0], parts[1], year));
                    }
                }
            }
        }

        public void SaveBooksToFile(string filePath)
        {
            var lines = books.Select(b => $"{b.Title}|{b.Author}|{b.Year}");
            File.WriteAllLines(filePath, lines);
        }
    }
}
