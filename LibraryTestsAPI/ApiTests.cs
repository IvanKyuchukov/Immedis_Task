using Newtonsoft.Json;
using RestSharp;
using System.Net;

namespace LibraryTestsAPI
{
    public class ApiTests
    {
        private RestClient client;
        private const string url = "http://qa-task.immedis.com/api";
        private RestRequest request;

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(url);
        }

        [Test]
        public void getAllUsers()
        {
            this.request = new RestRequest(url+ "/users/");
            var response = this.client.Execute(request);            
            var users = JsonConvert.DeserializeObject<List<User>>(response.Content);


            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.That(users.Count > 1000);
        }

        [Test]
        public void getUserByID()
        {
            this.request = new RestRequest(url + "/users/1428");
            var response = this.client.Execute(request);
            var user = JsonConvert.DeserializeObject<User>(response.Content);


            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.That(user.id.ToString(), Is.EqualTo("1428"));
            Assert.That(user.name, Is.EqualTo("user-250390853"));            
            Assert.That(user.booksTaken.Count(), Is.EqualTo(2));
        }

        [Test]
        public void getAllBooks()
        {
            this.request = new RestRequest(url + "/books/");
            var response = this.client.Execute(request);
            var books = JsonConvert.DeserializeObject<List<Book>>(response.Content);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);     
            Assert.That(books.Count > 400);
        }

        [Test]
        public void getBookByID()
        {
            this.request = new RestRequest(url + "/books/1525");
            var response = this.client.Execute(request);
            var book = JsonConvert.DeserializeObject<Book>(response.Content);
            BooksTaken booksTaken = new BooksTaken();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.That(book.id.ToString(), Is.EqualTo("1525"));
            Assert.That(book.name.ToString(), Is.EqualTo("Book-1425534574"));
            Assert.That(book.author.ToString(), Is.EqualTo("Joanne Rowling"));
            Assert.That(book.genre.ToString(), Is.EqualTo("Fantasy"));
            Assert.That(book.quontity == 100);
            Assert.That(book.booksTaken.Count(), Is.EqualTo(1));
        }

        [Test]
        public void getAllTakenBooks()
        {
            this.request = new RestRequest(url + "/getbook/");
            var response = this.client.Execute(request);
            var bookTaken = JsonConvert.DeserializeObject<List<BooksTaken>>(response.Content);

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);           
            Assert.That(bookTaken.Count > 20);
        }

        [Test]
        public void getBooksTakenByID()
        {
            this.request = new RestRequest(url + "/getbook/549");
            var response = this.client.Execute(request);
            var bookTaken = JsonConvert.DeserializeObject<BooksTaken>(response.Content);
            BooksTaken booksTaken = new BooksTaken();

            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Assert.That(bookTaken.id.ToString(), Is.EqualTo("549"));
            Assert.That(bookTaken.userId.ToString(), Is.EqualTo("1055"));
            Assert.That(bookTaken.bookId.ToString(), Is.EqualTo("969"));
            Assert.That(bookTaken.dateTaken, Is.EqualTo("2023-03-08T10:46:25.154+00:00"));
            Assert.That(bookTaken.book == null);
            Assert.That(bookTaken.user == null);
        }
    }
}