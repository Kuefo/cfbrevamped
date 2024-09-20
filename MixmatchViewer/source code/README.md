# Ncaa14MixmatchViewer
A utility to view and edit uniform mixmatch files for NCAA14.

This is a C# Windows Forms Application that you can build using Visual Studio.

![image](https://user-images.githubusercontent.com/47217759/149250817-8ebbce32-e3bf-4e15-ac84-f25994fc69c9.png)

# How to use
- Launch the application.
- From the File menu, select Open.
- Choose a valid mixmatch XML file from the dialog and click Open. Data will load into the tables.

## Viewing
You can select a preset name from the list on the left to filter the uniform parts to only those belonging to the preset.
To reset the filter, click a blank spot under the Presets list.
To show additional options for jerseys, click the "Show Advanced Jersey Params" button. You can also press this again to hide the same params. Even when they are hidden, they will still be written to the file when you save.

## Editing
You can edit any of the cells by clicking into them and typing. The last row of each table is blank; when you enter data in there, it will insert a new row.
There are also shortcuts for highlighted cells to simplify editing:
- Ctrl+C: Copies the currently selected cells (only do this for cells in the same row)
- Ctrl+V: Pastes copied cells into the row starting at the selected cell (make sure only one cell is selected/blue before pasting)
- Ctrl+D: Deletes the selected cells (only do this for cells in the same row)
- Delete/Backspace: Same as above
- Ctrl+R: Selects all cells in the currently selected row, including any hidden cells
- Ctrl+O: Opens a new file
- Ctrl+S: Saves the file

Any rows that have a blank first column will not be written. This program does not do any kind of validation; make sure everything you enter is valid data.
You can have two or more of this application open at once and copy/paste cells between them.

## Saving
- From the File menu, select Save As.
- Enter in a file name or select an existing file.
- Click Save.

Your saved file should now be ready to be used in the AST editor.
