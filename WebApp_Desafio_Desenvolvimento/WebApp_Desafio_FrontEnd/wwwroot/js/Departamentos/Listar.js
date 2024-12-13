$(document).ready(function () {

    var table = $('#dataTables-Departamentos').DataTable({
        paging: false,
        ordering: false,
        info: false,
        searching: false,
        processing: true,
        serverSide: true,
        ajax: config.contextPath + 'Departamentos/Datatable',
        columns: [
            { data: 'ID' },
            { data: 'Descricao', title: 'Descrição' },
        ],
        createdRow: function (row, data, dataIndex) {
            // Adicionando o data-id na linha
            $(row).attr('data-id', data.ID);
        }
    });

    // Evento click para selecionar a linha
    $('#dataTables-Departamentos tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        } else {
            table.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });

    // Evento para duplo clique
    $('#dataTables-Departamentos tbody').on('dblclick', 'tr', function () {
        var departamentoId = $(this).data('id');  // Pega o ID do departamento

        // Redireciona para a página de edição, passando o ID do departamento
        window.location.href = '/Departamentos/Editar/' + departamentoId;
    });

    $('#btnAdicionar').click(function () {
        window.location.href = config.contextPath + 'Departamentos/Cadastrar';
    });

    $('#btnEditar').click(function () {
        var data = table.row('.selected').data();
        if (!data) {
            Swal.fire({
                text: "Por favor, selecione um departamento para editar.",
                icon: "warning",
                confirmButtonText: 'OK'
            });
            return;
        }
        window.location.href = config.contextPath + 'Departamentos/Editar/' + data.ID;
    });

    // Botão Excluir Departamento
    $('#btnExcluir').click(function () {
        var data = table.row('.selected').data();
        if (!data) {
            Swal.fire({
                text: "Por favor, selecione um departamento para excluir.",
                icon: "warning",
                confirmButtonText: 'OK'
            });
            return;
        }

        let idRegistro = data.ID;

        Swal.fire({
            text: "Tem certeza de que deseja excluir o departamento: " + data.Descricao + " ?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: 'Sim, excluir',
            cancelButtonText: 'Cancelar'
        }).then(function (result) {
            if (result.value) {
                $.ajax({
                    url: config.contextPath + 'Departamentos/Excluir/' + idRegistro,
                    type: 'DELETE',
                    contentType: 'application/json',
                    error: function (result) {
                        Swal.fire({
                            text: result.responseText || "Erro ao excluir o departamento.",
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    },
                    success: function (result) {
                        Swal.fire({
                            icon: result.Type,
                            title: result.Title,
                            text: result.Message,
                        }).then(function () {
                            table.draw();
                        });
                    }
                });
            } else {
                console.log("Cancelou a exclusão.");
            }
        });
    });

    $('#btnRelatorio').click(function () {
        window.location.href = config.contextPath + 'Departamentos/Report';
    });

});
