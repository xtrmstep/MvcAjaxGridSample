This branch contains the code which describe an issue with POST operations with updating some controls. The code is located in the ...\Views\Home\_GridViewBooks.cshtml, in the end of the file.

The issue is that event if value of Model.Paging.PageIndex is changed before rendering the partial view, it is remains the same always (=1) and label and hidden input shows 1. At the same time the grid data is changes according ot the index whta it should be at the moment. Very strange and I don't not waht exactly causes this issue.

The code of this branch is functional and stored to have an example of the issue for investigation.
