  'use strict';
$(document).ready(function() {
      var simple = $('#simpletable').DataTable();

      var advance = $('#searchable').DataTable( {
      dom: 'Bfrtip',
      buttons: [
        'copy', 'csv', 'excel', 'pdf', 'print'
          ],
          "paging": false,
    } );


// Setup - add a text input to each footer cell
    $('#searchable tfoot th').each( function () {
        var title = $(this).text();
        $(this).html( '<div class="md-input-wrapper"><input type="text" class="text form-control" style="width:100px" placeholder="Search '+title+'" /></div>' );
    } );
      // Apply the search
    advance.columns().every( function () {
        var that = this;
 
        $('.text', this.footer()).on('keyup change', function () {
            if ( that.search() !== this.value ) {
                that
                    .search( this.value )
                    .draw();
            }
        } );
    } );

    

    } );