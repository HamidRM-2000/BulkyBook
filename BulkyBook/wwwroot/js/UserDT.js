var dataTable;

$(document).ready(function () {
   LoadTable();
});

function LoadTable() {
   dataTable = $("#Dtable").DataTable({
      "ajax": {
         "url": "/admin/users/getall",
      },
      "columns": [
         { "data": "name", "width": "15%" },
         { "data": "city", "width": "15%" },
         { "data": "state", "width": "15%" },
         { "data": "company.name", "width": "15%" },
         { "data": "email", "width": "15%" },
         {
            "data": {
               i: "id",
               lockoutEnd: "lockOutEnd"
            },
            "render": function (data) {
               var today = new Date().getTime();
               var lockout = new Date(data.lockoutEnd).getTime();
               if (lockout > today) {
                  //user is currently locked
                  return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-outline-danger" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock-open"></i>  Unlock
                                </a>
                            </div>
                           `;
               }
               else {
                  return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-outline-success" style="cursor:pointer; width:100px;">
                                    <i class="fas fa-lock"></i>  Lock
                                </a>
                            </div>
                           `;
               }

            }, "width": "25%"
         }
      ]
   });
}
function LockUnlock(id) {

   $.ajax({
      type: "POST",
      url: '/Admin/Users/LockUnlock',
      data: JSON.stringify(id),
      contentType: "application/json",
      success: function (data) {
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
         });
         Toast.fire({
            icon: data.success ? "success" : "error",
            title: data.message
         });
         dataTable.ajax.reload();
      }
   });
}