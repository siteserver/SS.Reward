var swal2 = swal.mixin({
  confirmButtonClass: 'btn btn-primary',
  cancelButtonClass: 'btn btn-default ml-2',
  buttonsStyling: false,
});

swal2.success = function(title) {
  swal2({
    toast: true,
    type: 'success',
    title: title,
    showConfirmButton: false,
    timer: 2000
  });
};