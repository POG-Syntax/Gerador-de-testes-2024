﻿using GeradorDeTestes.WinApp._1___Módulo_Compartilado;
using GeradorDeTestes.WinApp._2___Módulo_Disciplina;
using GeradorDeTestes.WinApp._2___Módulo_Disciplinas;
using GeradorDeTestes.WinApp._3___Módulo_Matérias;
using GeradorDeTestes.WinApp._5___Módulo_Questões;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeradorDeTestes.WinApp._4___Módulo_Testes
{
    public class ControladorTestes : ControladorBase, IControladorVisualizar
    {

        private IRepositorioTestes repositorioTeste;
        private TabelaTesteControl tabelaTeste;

        public IRepositorioDisciplina repositorioDisciplina;
        public IRepositorioQuestoes repositorioQuestao;
        public IRepositorioMateria repositorioMateria;

        public ControladorTestes() { } // Ctor para deserialização;

        public ControladorTestes(IRepositorioTestes testeRepositorio, IRepositorioDisciplina disciplinaRepositorio, IRepositorioMateria materiaRepositorio, IRepositorioQuestoes questoesRepositorio)
        {
            repositorioTeste = testeRepositorio;
            repositorioDisciplina = disciplinaRepositorio;
            repositorioMateria = materiaRepositorio;
            repositorioQuestao = questoesRepositorio;
        }
        public override string TipoCadastro { get { return "Testes"; } }

        public override string ToolTipAdicionar { get { return "Cadastrar um novo teste"; } }

        public override string ToolTipEditar { get { return "Editar um teste existente"; } }

        public override string ToolTipExcluir { get { return "Excluir um teste existente"; } }

        public string ToolTipVisualizar { get { return "Visualziar um teste"; } }

        public string ToolTipGerarTestePdf { get { return "Gerar um teste"; } }

        public override void Adicionar()
        {

            TelaTesteForm telaTeste = new TelaTesteForm(repositorioDisciplina, repositorioMateria, repositorioQuestao);

            CarregarDisciplinas(telaTeste);

            DialogResult resultado = telaTeste.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            Teste novoTeste = telaTeste.Teste;

            List<Teste> testes = repositorioTeste.SelecionarTodos();

            foreach (var teste in testes)
            {
                if (teste.Titulo.ToLower() == novoTeste.Titulo.ToLower())
                {

                    TelaPrincipalForm
                        .Instancia
                        .AtualizarRodape($"Já existe um teste com este nome.");
                    return;
                }

                repositorioTeste.Cadastrar(novoTeste);

                CarregarTestes();

                TelaPrincipalForm
                .Instancia
                .AtualizarRodape($"O registro \"{novoTeste.Titulo}\" foi criado com sucesso!");
            }
        }

        public override void Editar()
        {
            TelaTesteForm telaTeste = new TelaTesteForm(repositorioDisciplina, repositorioMateria, repositorioQuestao);

            CarregarDisciplinas(telaTeste);
            CarregarMaterias(telaTeste);
            //CarregarQ(telaTeste);

            int idSelecionado = tabelaTeste.ObterRegistroSelecionado();

            Teste Selecionado = repositorioTeste.SelecionarPorId(idSelecionado);

            if (Selecionado == null)
            {
                MessageBox.Show(
                 "Não é possível realizar esta ação sem um registro selecionado.",
                 "Aviso",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning
                 ); return;
            }
            telaTeste.Teste = Selecionado;

            DialogResult resultado = telaTeste.ShowDialog();

            if (resultado != DialogResult.OK) return;

            Teste testeEditado = telaTeste.Teste;

            repositorioTeste.Editar(testeEditado.Id, testeEditado);

            CarregarTestes();

        }
        public override void Excluir()
        {
            int idSelecionado = tabelaTeste.ObterRegistroSelecionado();

            Teste testeSelecionado = repositorioTeste.SelecionarPorId(idSelecionado);

            if (testeSelecionado == null)
            {
                TelaPrincipalForm
                          .Instancia
                          .AtualizarRodape($"Não é possível realizar esta ação sem um registro selecionado.");
                return;
            }

            DialogResult resultado = MessageBox.Show(
               $"Você deseja realmente excluir o registro \"{testeSelecionado.Id}\"?",
               "Confirmar Exclusão",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Warning);

            if (resultado != DialogResult.Yes) return;

            repositorioTeste.Excluir(testeSelecionado.Id);

            CarregarTestes();

            TelaPrincipalForm
                 .Instancia
                 .AtualizarRodape($"O registro \"{testeSelecionado.Titulo}\" foi excluído com sucesso!");
        }

        public override UserControl ObterListagem()
        {
            if (tabelaTeste == null)
                tabelaTeste = new TabelaTesteControl();

            CarregarTestes();

            return tabelaTeste;
        }
        private void CarregarTestes()
        {
            List<Teste> testes = repositorioTeste.SelecionarTodos();

            tabelaTeste.AtualizarRegistros(testes);
        }
        void CarregarDisciplinas(TelaTesteForm telaTeste)
        {
            List<Disciplinas> disciplinas = repositorioDisciplina.SelecionarTodos();

            telaTeste.MostrarDisciplinas(disciplinas);

        }

        void CarregarMaterias(TelaTesteForm telaTeste)
        {
            List<Materias> materias = repositorioMateria.SelecionarTodos();


            telaTeste.MostrarMaterias(materias);
        }

        public void Visualizar()
        {
            int idSelecionado = tabelaTeste.ObterRegistroSelecionado();

            Teste testeSelecionado =
                repositorioTeste.SelecionarPorId(idSelecionado);

            if (testeSelecionado == null)
            {
                TelaPrincipalForm
                    .Instancia
                    .AtualizarRodape($"Não é possível realizar esta ação sem um registro selecionado.");
                return;
            }

            TelaVisualizarTesteForm tela = new TelaVisualizarTesteForm(testeSelecionado);

            tela.ShowDialog();
        }

        public void GerarTeste()
        {
            int idSelecionado = tabelaTeste.ObterRegistroSelecionado();

            Teste testeSelecionado = repositorioTeste.SelecionarPorId(idSelecionado);

            if (testeSelecionado == null)
            {

                TelaPrincipalForm
                    .Instancia
                    .AtualizarRodape($"Não é possível realizar esta ação sem um registro selecionado.");
                return;
            }

            TelaGerarPDFForm telaGerarPDF = new TelaGerarPDFForm(testeSelecionado, repositorioDisciplina.SelecionarTodos(), repositorioMateria.SelecionarTodos(), repositorioQuestao.SelecionarTodos());

            DialogResult resultado = telaGerarPDF.ShowDialog();

            if (resultado != DialogResult.OK)
                return;

            string caminho = telaGerarPDF.Caminho;

            TelaPrincipalForm.Instancia.AtualizarRodape($"O PDF foi gerado com sucesso em: {caminho}");
        }
    }
}
