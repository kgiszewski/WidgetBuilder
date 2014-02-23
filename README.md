WidgetBuilder for Umbraco v6
=

### What is it? ###

[Video Demo](http://www.youtube.com/watch?feature=player_embedded&v=GMCVmM3RN8o)

A blog about it from [Maria Lind](http://inaboxdesign.dk/blog/widget-builder-for-umbraco/)

Widget Builder is on-the-fly fieldset management and gives editors the flexibility to add what they need when they need it.

Comes with several native property types and is compatible with some 3rd-party property editors like [DAMP](http://our.umbraco.org/projects/backoffice-extensions/digibiz-advanced-media-picker)

- Allows editors to add fields at run-time.
- Set permissions on a per Widget or a per Widget Property level. 
- Use the Umbraco data type editor to setup your custom widget.
- Attach custom JS/CSS files to your Widget.

Note: This is not compatible with Umbraco v7.

### Sunsetting ###

Note that this project is sunsetting due to incompatibility with Umbraco 7.  This means no active development is planned but pull-requests and the occassional bug fix with continue.

### Known Issues ###

Widget Builder isn't perfect, here is a list of known issues:

- WB does not play nice with the 'Related Links' data type. Please use the uComponents Multi-Url Picker instead.
- WB's RTE (TinyMCE) implementation has an issue with 'Insert Macro'. At this time it does not work.
- WB's RTE (TinyMCE) implementation has an issue with the 'styles' dropdown being misplaced at times. There is no fix at this time.
- WB's RTE (TinyMCE) implementation had an issue with Umbraco v6.1.x when sorted. There is now a fix! Replace your /umbraco/Plugins/WidgetBuilder/WidgetBuilder.js with this.
