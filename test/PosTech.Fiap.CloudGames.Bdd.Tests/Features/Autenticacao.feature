# language: pt
# Requisito (Desafio Fase 1 · RT-10): "aplicar TDD ou BDD em pelo menos um dos módulos".
# Este é o módulo escolhido para BDD — autenticação (cadastro + login).
Funcionalidade: Autenticação de usuários
  Para acessar a plataforma FIAP Cloud Games
  Como um jogador
  Quero me cadastrar e autenticar com segurança

  Cenário: Cadastro com senha forte seguido de login bem-sucedido
    Dado que não existe usuário cadastrado com o e-mail "jogador@cloudgames.com"
    Quando eu cadastro o usuário "Jogador" com e-mail "jogador@cloudgames.com" e senha "Str0ng!Pass"
    Então o cadastro é realizado com sucesso
    E o login com e-mail "jogador@cloudgames.com" e senha "Str0ng!Pass" retorna um token válido

  Cenário: Cadastro com senha fraca é rejeitado
    Quando eu tento cadastrar o usuário "Fraco" com e-mail "fraco@cloudgames.com" e senha "123"
    Então o cadastro é rejeitado por senha inválida

  Cenário: Login com senha incorreta é negado
    Dado que existe um usuário "Maria" com e-mail "maria@cloudgames.com" e senha "Str0ng!Pass"
    Quando eu tento autenticar com e-mail "maria@cloudgames.com" e senha "SenhaErrada1!"
    Então a autenticação é negada
