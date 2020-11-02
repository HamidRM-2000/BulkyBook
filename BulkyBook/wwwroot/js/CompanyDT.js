var dataTable;

$(document).ready(function () {
   LoadTable();
});

function LoadTable() {
   dataTable = $("#Dtable").DataTable({
      "ajax": {
         "url": "/admin/companies/getall",
      },
      "columns": [
         { "data": "name", "width": "18%" },
         { "data": "city", "width": "18%" },
         { "data": "state", "width": "17%" },
         { "data": "postalCard", "width": "17%" },
         {
            "data": "comId",
            "render": function (data) {
               return `
                        <div class="text-center">
                           <a  class="btn btn-outline-primary" href="/admin/companies/details/${data}">
                              <i class="fa fa-info-circle"></i>&nbsp Details
                           </a>
                           <a class="btn btn-outline-warning" href="/admin/companies/edit/${data}">
                              <i class="fa fa-edit"></i>&nbsp Edit
                           </a>
                           <a onclick="LoadAlert(${data})" class="btn btn-outline-danger">
                              <i class="fa fa-trash"></i>&nbsp Delete
                           </a>
                        </div>
                      `;
            }, "width": "30%"
         }
      ]
   });
}
function LoadAlert(id) {
   Swal.fire({
      title: 'Are you sure to delete?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#f33d3a',
      cancelButtonColor: '#028ec3',
      confirmButtonText: 'Yes, delete it!'
   }).then((result) => {
      if (result.isConfirmed) {
         $.ajax({
            url: `/admin/companies/delete/${id}`,
            type: "delete"
         }).done(function (res) {
            const Toast = Swal.mixin({
               toast: true,
               position: 'top-end',
               showConfirmButton: false,
               timer: 3000,
               timerProgressBar: true,
               onOpen: (toast) => {
                  toast.addEventListener('mouseenter', Swal.stopTimer)
                  toast.addEventListener('mouseleave', Swal.resumeTimer)
               }
            })

            Toast.fire({
               icon: res.success ? "success" : "error",
               title: res.message
            })
            dataTable.ajax.reload();
         });
      }
   })
}