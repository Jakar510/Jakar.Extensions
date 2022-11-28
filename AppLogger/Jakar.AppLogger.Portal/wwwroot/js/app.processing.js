
function showProcessLoading( target ) {
    $('#processing').css('visibility', 'visible');
    if (hasProcessingClass(target)) {
        $('#loading').css('visibility', 'visible');
        $('#mask').css('visibility', 'hidden');
    }
    else {
        $('#loading').css('visibility', 'hidden');
        $('#mask').css('visibility', 'visible');
        setTimeout(function() { $('#mask').css('cursor', 'wait'); }, 1000);
    }
}

function hideProcessLoading() {
    $('#processing').css('visibility', 'hidden');
    $('#loading').css('visibility', 'hidden');
    $('#mask').css('visibility', 'hidden');
    $('#mask').css('cursor', '');
}

function hasProcessingClass( target ) {
    var elementName;
    if (typeof ( target ) == 'string') {
        elementName = target;
    }
    else {
        Object.keys(DotNetElements)
            .find(function( key ) {
                if (DotNetElements[key] == target) {
                    elementName = key;
                }
            });
    }
    var result = false;
    if (elementName) {
        $('.processing')
            .each(function() {
                $.each(this.attributes,
                    function( i, attr ) {
                        if (attr.value.includes(elementName)) {
                            result = true;
                        }
                    });
            });
    }
    return result;
}
