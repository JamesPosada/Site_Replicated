/*
  Changes a state/region's options when the bound country's option is changed.
  Sample Usage:
    <select id="state">...</select>
    <select id="country" data-region-bind="#state">...</select>
*/
$(function () {
    $('[data-region-bind]').on('change', document, function (event) {

        var $this = $(this),
            country = $this.val(),
            region = $this.data('region-bind'),
            $region = $((!region.contains('#')) ? '#' + region : region);

        $.ajax({
            url: app.path + "/app/getregions/" + country,
            type: 'GET',
            cache: true,
            success: function (response) {

                if (response.success) {
                    var html = '',
                        regions = response.regions;

                    // Assemble the new region options
                    for (i in regions) {
                        var region = regions[i];
                        html += '<option value="{0}">{1}</option>'.format(
                            region.RegionCode,
                            region.RegionName);
                    }

                    // Populate the regions
                    $region.html(html);
                }
            }
        });
    });
});