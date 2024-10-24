$(function () {
  $('.delete-company').on('click', function (e) {
    e.preventDefault();
    const id = $(this).data('id');

    Swal.fire({
      title: 'Are you sure?',
      text: 'You won\'t be able to revert this!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, delete it!'
    }).then(async (result) => {
      if (result.isConfirmed) {
        const res = await fetch(`/admin/company/delete/${id}`, { method: 'DELETE' });
        const result = await res.json();        
        toastr.success(result.message);
        location.reload();
      }
    })
  })
});
