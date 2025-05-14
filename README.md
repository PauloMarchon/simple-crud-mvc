# CrudMVC

Crud simples utilizando ASP.NET Core MVC e comunicacao com banco de dados Oracle para realizar as operacoes diretamente no banco de dados atraves de procedures.

<h3>Procedures do Banco de Dados Oracle</h3>
<h4>Cadastro de Usuario:</h4>

```
create or replace NONEDITIONABLE PROCEDURE sp_criar_novo_usuario (
    p_id    IN usuarios.id%TYPE,    
    p_nome  IN usuarios.nome%TYPE,  
    p_email IN usuarios.email%TYPE, 
    p_cargo IN usuarios.cargo%TYPE 
)
IS
BEGIN
    INSERT INTO usuarios (id, nome, email, cargo)
    VALUES (p_id, p_nome, p_email, p_cargo);

    COMMIT;

    DBMS_OUTPUT.PUT_LINE('Usuário "' || p_nome || '" criado com sucesso com ID: ' || RAWTOHEX(p_id));

EXCEPTION
    WHEN DUP_VAL_ON_INDEX THEN 
        DBMS_OUTPUT.PUT_LINE('Erro: Já existe um usuário com o ID ' || RAWTOHEX(p_id) || ' ou outro valor único duplicado.');
    WHEN OTHERS THEN 
        DBMS_OUTPUT.PUT_LINE('Erro ao criar usuário: ' || SQLCODE || ' - ' || SQLERRM);
END sp_criar_novo_usuario;

```
<h4>Buscar Usuario:</h4>

```
create or replace NONEDITIONABLE PROCEDURE SP_BUSCAR_USUARIO_POR_ID (
    p_id  IN RAW,
    p_cursor      OUT SYS_REFCURSOR
)
IS
BEGIN
    OPEN p_cursor FOR
    SELECT
        id, 
        nome,
        email,
        cargo
    FROM
        usuarios
    WHERE
        id = p_id;

EXCEPTION
    WHEN OTHERS THEN
        IF p_cursor%ISOPEN THEN
            CLOSE p_cursor;
        END IF;
        RAISE;
END;
```

<h4>Listar Usuarios:</h4>

```
create or replace NONEDITIONABLE PROCEDURE SP_LISTAR_USUARIOS (
    P_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_CURSOR FOR
        SELECT
            ID,       
            NOME,     
            EMAIL,   
            CARGO     
        FROM
            USUARIOS;   
END SP_LISTAR_USUARIOS;
```

<h4>Editar Usuario:</h4>

```
create or replace NONEDITIONABLE PROCEDURE SP_EDITAR_USUARIO_POR_ID(
    p_usuario_id     IN RAW,
    p_novo_nome      IN VARCHAR2 DEFAULT NULL, 
    p_novo_email     IN VARCHAR2 DEFAULT NULL, 
    p_novo_cargo     IN VARCHAR2 DEFAULT NULL,
   
    p_linhas_afetadas OUT NUMBER 
)
IS
    v_count NUMBER := 0; 
BEGIN
    UPDATE usuarios
    SET
        nome = NVL(p_novo_nome, nome), 
        email = NVL(p_novo_email, email), 
        cargo = NVL(p_novo_cargo, cargo)
    WHERE
        id = p_usuario_id; 

    v_count := SQL%ROWCOUNT;

    p_linhas_afetadas := v_count;
     COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        p_linhas_afetadas := 0; -- Em caso de erro, indica 0 linhas afetadas
        RAISE; 
END;
```

<h4>Remover Usuario:</h4>

```
create or replace NONEDITIONABLE PROCEDURE SP_REMOVER_USUARIO_POR_ID (
    p_id  IN RAW,
    p_linhas_afetadas OUT NUMBER
)
IS
    v_count NUMBER := 0;
BEGIN
    DELETE FROM usuarios 
    WHERE id = p_id; 

    v_count := SQL%ROWCOUNT;

    p_linhas_afetadas := v_count;

    COMMIT;

EXCEPTION
    WHEN OTHERS THEN
        p_linhas_afetadas := 0;
END;
```
