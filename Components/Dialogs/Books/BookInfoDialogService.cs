namespace BookHeaven.Reader.Components.Dialogs.Books;

public class BookInfoDialogService
{
    public event Action<Book>? OnShow;
    
    public void Show(Book book)
    {
        OnShow?.Invoke(book);
    }
}