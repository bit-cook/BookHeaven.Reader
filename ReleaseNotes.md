# 🚀 New UI for the Local books tab!
I actually added in [the roadmap](https://github.com/orgs/BookHeaven/discussions/2) that I wanted to have better search for the server tab, and I kinda did by adding a searchbar, but it's not really all I wanted to do.

Ultimately, despite not even being in my plans, the local tab is the one that annoyed me the most on a regular basis, so I decided to redesign it first instead. And here it is, finally!

Now there's only two tabs instead of four: Home and Shelf.

Home is divided into two sections: Reading and New.<br/>
The Reading section shows the books that you are currently reading (sorted by last read, finally), and the New section shows the books that you have yet to start reading.

Shelf is basically the same as the old "All" tab, but it now includes a badge with the amount of books that you have.

Aaand there is a searchbar as well.

Check the [readme](https://github.com/BookHeaven/BookHeaven.Reader) for the updated juicy screenshots.
<hr/>

# Improvements
- The covers of the books in the Local tab are no longer loaded as base64 images, which can reduce the memory usage by a considerable amount depending on the case.
- The context menu for books and apps no longer renders outside the viewport for a split second when opening it near the right or bottom edges.

# Fixes
- The remote tab won't reset to the first page anymore after downloading a book.
- The retry button after a failed connection in the remote tab now works properly.





