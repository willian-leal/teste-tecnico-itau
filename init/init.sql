-- ==============================
-- Parte 1 – Modelagem Relacional
-- ==============================

CREATE TABLE usuario (
    id INT PRIMARY KEY AUTO_INCREMENT,
    nome VARCHAR(100) NOT NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    percentual_corretagem DECIMAL(5,2) NOT NULL
);

CREATE TABLE ativo (
    id INT PRIMARY KEY AUTO_INCREMENT,
    codigo VARCHAR(10) NOT NULL UNIQUE,
    nome VARCHAR(100) NOT NULL
);

CREATE TABLE operacoes (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    quantidade INT NOT NULL,
    preco_unitario DECIMAL(10,2) NOT NULL,
    tipo_operacao ENUM('COMPRA', 'VENDA') NOT NULL,
    corretagem DECIMAL(10,2) NOT NULL,
    data_hora DATETIME NOT NULL,
    FOREIGN KEY (usuario_id) REFERENCES usuario(id),
    FOREIGN KEY (ativo_id) REFERENCES ativo(id)
);

CREATE TABLE cotacao (
    id INT PRIMARY KEY AUTO_INCREMENT,
    ativo_id INT NOT NULL,
    preco_unitario DECIMAL(10,2) NOT NULL,
    data_hora DATETIME NOT NULL,
    FOREIGN KEY (ativo_id) REFERENCES ativo(id)
);

CREATE TABLE posicao (
    id INT PRIMARY KEY AUTO_INCREMENT,
    usuario_id INT NOT NULL,
    ativo_id INT NOT NULL,
    quantidade INT NOT NULL,
    preco_medio DECIMAL(10,2) NOT NULL,
    pnl DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (usuario_id) REFERENCES usuario(id),
    FOREIGN KEY (ativo_id) REFERENCES ativo(id)
);

-- ======================
-- Parte 2 – Índices
-- ======================

CREATE INDEX idx_oper_usuario_ativo_data ON operacoes (usuario_id, ativo_id, data_hora);
CREATE INDEX idx_cot_ativo_data ON cotacao (ativo_id, data_hora DESC);
CREATE INDEX idx_pos_usuario_ativo ON posicao (usuario_id, ativo_id);

-- ======================================
-- Parte 2 – Procedure e Trigger (P&L)
-- ======================================

DELIMITER //

CREATE PROCEDURE sp_update_posicao (
    IN p_ativo_id INT,
    IN p_preco_mercado DECIMAL(10,2)
)
BEGIN
    UPDATE posicao
    SET pnl = (p_preco_mercado - preco_medio) * quantidade
    WHERE ativo_id = p_ativo_id;
END;
//

DELIMITER ;

CREATE TRIGGER tr_cotacao_ai
AFTER INSERT ON cotacao
FOR EACH ROW
CALL sp_update_posicao(NEW.ativo_id, NEW.preco_unitario);
