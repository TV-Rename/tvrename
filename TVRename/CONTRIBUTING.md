# How to contribute

I'm really glad you're reading this, because we need volunteer developers to help this project come to fruition.

Here are some important resources:

  * [TV Rename website](http://www.tvrename.com/)
  * Join our [support forum](https://groups.google.com/forum/#!forum/tvrename)
  * Vote on [feature suggestions](https://tvrename.featureupvote.com/)

## Testing

Please test all changes before committing!

## Submitting changes

Please send a [GitHub Pull Request to TV-Rename](https://github.com/TV-Rename/tvrename/pull/new/master) with a clear list of what you've done (read more about [pull requests](http://help.github.com/pull-requests/)). When you send a pull request. Please follow our coding conventions (below) and make sure all of your commits are atomic (one feature per commit).

Always write a clear log message for your commits. One-line messages are fine for small changes, but bigger changes should look like this:

    $ git commit -m "A brief summary of the commit
    > 
    > A paragraph describing what changed and its impact."

## Coding conventions

Start reading our code and you'll get the hang of it. We optimize for readability:

  * We use [EditorConfig](http://editorconfig.org/) for this project, please install the extention for your editor.
  * We indent using four spaces (soft tabs), EditorConfig will take care of this.
  * We always put spaces after list items and method parameters (`[1, 2, 3]`, not `[1,2,3]`), around operators (`x += 1`, not `x+=1`), and around hash arrows.
  * This is open source software. Consider the people who will read your code, and make it look nice for them. It's sort of like driving a car: Perhaps you love doing donuts when you're alone, but with passengers the goal is to make the ride as smooth as possible.
