$(function () {
  const query = location.search//?status=all
  const param = new URLSearchParams(query).get('status')//all
  $('.btn-group a').each(function () {
    const str = $(this).text().trim()
    if (str === param) {
      $(this).addClass('active')
    }
  })
});
