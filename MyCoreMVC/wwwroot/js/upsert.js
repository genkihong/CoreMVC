$(function () {
  $('#Product_Images').on('change', function () {
    //console.log(this.files);    
    //const files = $(this)[0].files;
    $('#prevImage .col-2').remove();
    if (this.files.length > 0) {
      for (let i = 0; i < this.files.length; i++) {
        const file = this.files[i];
        const content = `<div class="col-2">
                          <img src="${URL.createObjectURL(file)}" alt="" class="img-thumbnail" />
                        </div>`;
        $('#prevImage').append(content);
      }
    }
  });
  // 預覽圖片
  //$('#Product_Image').on('change', function () {
  //  const file = this.files[0];
  //  if (file) {
  //    // 使用 createObjectURL 產生圖片url
  //    const objectURL = URL.createObjectURL(file);
  //    $('#prevImage').attr('src', objectURL);
  //    $('#prevImage').removeClass('d-none');
  //    $('.bi-image').addClass('d-none');
  //  } else {
  //    $('#prevImage').addClass('d-none');
  //    $('.bi-image').removeClass('d-none');
  //  }
  //});
});

