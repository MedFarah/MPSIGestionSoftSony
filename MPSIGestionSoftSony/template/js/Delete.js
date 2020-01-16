function Delete(id) {
            //if (confirm('Are You Sure to Delete this Employee Record ?')) {
            swal({
                title: "Are you sure?",
                text: "Once deleted, you will not be able to recover this Panne!",
                icon: "warning",
                buttons: true,
                dangerMode: true,
            })
                .then((willDelete) => {
                    if (willDelete) {

                        $.ajax({
                            type: "POST",
                            url: "/Pannes/Delete/" + id

                        });
                        swal("Poof! Panne has been deleted!", {
                            icon: "success",

                        }).then(function () {
                            location.reload();
                        });
                        //location.reload();
                    } else {
                        swal("Panne is safe!");
                    }
                });
        }