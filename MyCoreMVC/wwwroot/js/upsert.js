$(function () {
  // 預覽圖片
  $('#Product_Image').on('change', function (e) {
    const file = this.files[0];
    if (file) {
      // 使用 createObjectURL 產生圖片url
      const objectURL = URL.createObjectURL(file);
      $('#prevImage').attr('src', objectURL);
      $('#prevImage').removeClass('d-none');
      $('.bi-image').addClass('d-none');
    } else {
      $('#prevImage').addClass('d-none');
      $('.bi-image').removeClass('d-none');
    }
  });
});

