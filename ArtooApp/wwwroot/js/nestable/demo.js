$(document).ready(function()
{

    // activate Nestable for list 1
    $('#nestable1').nestable({
        group: 1
    });
    
    // activate Nestable for list 2
    $('#nestable2').nestable({
        group: 1
    });

    var $expand = false;
    $('#nestable-menu').on('click', function(e)
    {
        if ($expand) {
            $expand = false;
            $('.ddd').nestable('expandAll');
        }else {
            $expand = true;
            $('.ddd').nestable('collapseAll');
        }
    });

    $('#nestable3').nestable();

});