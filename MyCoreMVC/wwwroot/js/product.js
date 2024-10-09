$(function () {
  let dataTable = null;
  //DataTable
  const loadDataTable = () => {
    dataTable = $('#tblData').DataTable({
      info: false,
      ordering: false,
      searching: false,
      paging: true,
      ajax: '/admin/product/getall',
      language: {
        url: '../lib/datatable/zh-HANT.json'
      },
      columns: [
        {
          data: 'title',
          className: 'align-middle'
        },
        {
          data: 'author',
          className: 'align-middle'
        },
        {
          data: 'category.name',
          className: 'align-middle'
        },
        {
          data: 'imageUrl',
          className: 'text-center',
          render(data) {
            return `<img src="/images/product/${data}" alt="${data}" width="100" />`
          }
        },
        {
          data: 'isbn',
          className: 'align-middle'
        },
        {
          data: 'listPrice',
          className: 'align-middle'
        },
        {
          data: 'id',
          className: 'align-middle text-end',
          render(data) {
            return ` <a href="/admin/product/upsert?id=${data}" class="btn btn-sm btn-primary">
                        <i class="bi bi-pencil-square me-1"></i>編輯
                     </a>
                     <a href="#" class="btn btn-sm btn-danger delete-product" data-id="${data}">
                        <i class="bi bi-trash-fill me-1"></i>刪除
                     </a>`
          },
        }
      ],
    });
  }
  // 刪除產品
  const deleteProduct = () => {
    $(document).on("click", ".delete-product", function (e) {
      e.preventDefault();
      //console.log($(this).data('id'))
      //console.log(this.getAttribute('data-id'))
      //console.log(this.dataset.id)
      const id = $(this).data('id');

      Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
      }).then(async (result) => {
        if (result.isConfirmed) {
          const res = await fetch(`/admin/product/delete/${id}`, { method: 'DELETE' });
          const result = await res.json()
          dataTable.ajax.reload();
          toastr.success(result.message);
        }
      })
    });
  }

  loadDataTable();
  deleteProduct();
});

