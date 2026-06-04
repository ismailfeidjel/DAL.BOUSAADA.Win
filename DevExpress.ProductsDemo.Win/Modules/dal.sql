-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Hôte : 127.0.0.1
-- Généré le : jeu. 04 juin 2026 à 16:44
-- Version du serveur : 10.4.32-MariaDB
-- Version de PHP : 8.0.30

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données : `dal`
--

-- --------------------------------------------------------

--
-- Structure de la table `administrative_procedures`
--

CREATE TABLE `administrative_procedures` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `administrative_procedures`
--

INSERT INTO `administrative_procedures` (`id`, `name`) VALUES
(5, 'CFM'),
(6, 'TS'),
(7, 'TS1'),
(8, 'إعانة'),
(2, 'إعلان'),
(4, 'دراسة'),
(1, 'دفتر'),
(3, 'منح');

-- --------------------------------------------------------

--
-- Structure de la table `communes`
--

CREATE TABLE `communes` (
  `id` int(11) NOT NULL,
  `daira_id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `communes`
--

INSERT INTO `communes` (`id`, `daira_id`, `name`) VALUES
(1, 1, 'ولتام');

-- --------------------------------------------------------

--
-- Structure de la table `dairas`
--

CREATE TABLE `dairas` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `dairas`
--

INSERT INTO `dairas` (`id`, `name`) VALUES
(1, 'بوسعادة');

-- --------------------------------------------------------

--
-- Structure de la table `domains`
--

CREATE TABLE `domains` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `domains`
--

INSERT INTO `domains` (`id`, `name`) VALUES
(5, 'الأشغال العمومية'),
(2, 'التربية'),
(1, 'التهيئة الحضرية'),
(4, 'الري'),
(3, 'الصحة');

-- --------------------------------------------------------

--
-- Structure de la table `lots`
--

CREATE TABLE `lots` (
  `id` int(11) NOT NULL,
  `project_id` int(11) NOT NULL,
  `lot_number` int(11) NOT NULL,
  `lot_name` varchar(255) NOT NULL,
  `contractor` varchar(255) DEFAULT NULL,
  `execution_duration` int(11) DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `physical_progress` decimal(5,2) DEFAULT 0.00,
  `administrative_procedure_id` int(11) DEFAULT NULL,
  `special_status1_id` int(11) DEFAULT NULL,
  `special_status2_id` int(11) DEFAULT NULL,
  `special_status3_id` int(11) DEFAULT NULL,
  `project_status_id` int(11) DEFAULT NULL,
  `notes` text DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `updated_by` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Structure de la table `programs`
--

CREATE TABLE `programs` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `programs`
--

INSERT INTO `programs` (`id`, `name`) VALUES
(1, 'ADSEC2025'),
(2, 'ADSEC2026');

-- --------------------------------------------------------

--
-- Structure de la table `projects`
--

CREATE TABLE `projects` (
  `id` int(11) NOT NULL,
  `operation_number` varchar(100) NOT NULL,
  `operation_name` text NOT NULL,
  `program_id` int(11) NOT NULL,
  `daira_id` int(11) NOT NULL,
  `commune_id` int(11) NOT NULL,
  `domain_id` int(11) NOT NULL,
  `sector_id` int(11) NOT NULL,
  `total_budget` decimal(18,2) DEFAULT 0.00,
  `registered_amount` decimal(18,2) DEFAULT 0.00,
  `consumed_amount` decimal(18,2) DEFAULT 0.00,
  `has_lots` tinyint(1) DEFAULT 0,
  `notes` text DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `updated_by` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `projects`
--

INSERT INTO `projects` (`id`, `operation_number`, `operation_name`, `program_id`, `daira_id`, `commune_id`, `domain_id`, `sector_id`, `total_budget`, `registered_amount`, `consumed_amount`, `has_lots`, `notes`, `created_at`, `updated_at`, `updated_by`) VALUES
(1, 'ADSEC-2025-001', 'تهيئة طريق بلدي', 2, 1, 1, 1, 1, 50000000.00, 50000000.00, 10000000.00, 0, 'عملية تجريبية', '2026-06-04 12:43:05', '2026-06-04 12:43:05', NULL);

-- --------------------------------------------------------

--
-- Structure de la table `project_details`
--

CREATE TABLE `project_details` (
  `project_id` int(11) NOT NULL,
  `contractor` varchar(255) DEFAULT NULL,
  `execution_duration` int(11) DEFAULT NULL,
  `start_date` date DEFAULT NULL,
  `physical_progress` decimal(5,2) DEFAULT 0.00,
  `administrative_procedure_id` int(11) DEFAULT NULL,
  `special_status1_id` int(11) DEFAULT NULL,
  `special_status2_id` int(11) DEFAULT NULL,
  `special_status3_id` int(11) DEFAULT NULL,
  `project_status_id` int(11) DEFAULT NULL,
  `notes` text DEFAULT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `updated_by` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `project_details`
--

INSERT INTO `project_details` (`project_id`, `contractor`, `execution_duration`, `start_date`, `physical_progress`, `administrative_procedure_id`, `special_status1_id`, `special_status2_id`, `special_status3_id`, `project_status_id`, `notes`, `created_at`, `updated_at`, `updated_by`) VALUES
(1, 'مؤسسة البناء الحديثة', 12, '2025-01-15', 35.00, 3, 1, 2, 1, 3, 'الأشغال جارية', '2026-06-04 12:43:18', '2026-06-04 12:43:18', NULL);

-- --------------------------------------------------------

--
-- Structure de la table `project_statuses`
--

CREATE TABLE `project_statuses` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `project_statuses`
--

INSERT INTO `project_statuses` (`id`, `name`) VALUES
(3, 'جارية'),
(1, 'غير مسجلة'),
(4, 'متوقفة'),
(6, 'مستلمة'),
(2, 'مسجلة'),
(7, 'مغلقة'),
(8, 'ملغاة'),
(5, 'منتهية');

-- --------------------------------------------------------

--
-- Structure de la table `sectors`
--

CREATE TABLE `sectors` (
  `id` int(11) NOT NULL,
  `name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `sectors`
--

INSERT INTO `sectors` (`id`, `name`) VALUES
(1, 'الأشغال العمومية'),
(3, 'التربية'),
(2, 'الري'),
(5, 'الشباب والرياضة'),
(4, 'الصحة');

-- --------------------------------------------------------

--
-- Structure de la table `special_status1`
--

CREATE TABLE `special_status1` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `special_status1`
--

INSERT INTO `special_status1` (`id`, `name`) VALUES
(3, 'دون'),
(2, 'غير مصنف'),
(1, 'مصنف');

-- --------------------------------------------------------

--
-- Structure de la table `special_status2`
--

CREATE TABLE `special_status2` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `special_status2`
--

INSERT INTO `special_status2` (`id`, `name`) VALUES
(1, 'للتسجيل'),
(2, 'للدراسة');

-- --------------------------------------------------------

--
-- Structure de la table `special_status3`
--

CREATE TABLE `special_status3` (
  `id` int(11) NOT NULL,
  `name` varchar(50) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `special_status3`
--

INSERT INTO `special_status3` (`id`, `name`) VALUES
(2, 'CF'),
(1, 'تداول');

-- --------------------------------------------------------

--
-- Structure de la table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `username` varchar(50) NOT NULL,
  `password_hash` varchar(255) NOT NULL,
  `full_name` varchar(150) DEFAULT NULL,
  `role` enum('admin','manager','editor','viewer') DEFAULT 'viewer',
  `is_active` tinyint(1) DEFAULT 1,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `users`
--

INSERT INTO `users` (`id`, `username`, `password_hash`, `full_name`, `role`, `is_active`, `created_at`) VALUES
(1, 'admin', '123', 'System Administrator', 'admin', 1, '2026-06-04 12:41:18');

--
-- Index pour les tables déchargées
--

--
-- Index pour la table `administrative_procedures`
--
ALTER TABLE `administrative_procedures`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `communes`
--
ALTER TABLE `communes`
  ADD PRIMARY KEY (`id`),
  ADD KEY `daira_id` (`daira_id`);

--
-- Index pour la table `dairas`
--
ALTER TABLE `dairas`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `domains`
--
ALTER TABLE `domains`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `lots`
--
ALTER TABLE `lots`
  ADD PRIMARY KEY (`id`),
  ADD KEY `administrative_procedure_id` (`administrative_procedure_id`),
  ADD KEY `special_status1_id` (`special_status1_id`),
  ADD KEY `special_status2_id` (`special_status2_id`),
  ADD KEY `special_status3_id` (`special_status3_id`),
  ADD KEY `updated_by` (`updated_by`),
  ADD KEY `idx_lot_project` (`project_id`),
  ADD KEY `idx_lot_status` (`project_status_id`);

--
-- Index pour la table `programs`
--
ALTER TABLE `programs`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `projects`
--
ALTER TABLE `projects`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `operation_number` (`operation_number`),
  ADD KEY `updated_by` (`updated_by`),
  ADD KEY `idx_project_program` (`program_id`),
  ADD KEY `idx_project_daira` (`daira_id`),
  ADD KEY `idx_project_commune` (`commune_id`),
  ADD KEY `idx_project_domain` (`domain_id`),
  ADD KEY `idx_project_sector` (`sector_id`);

--
-- Index pour la table `project_details`
--
ALTER TABLE `project_details`
  ADD PRIMARY KEY (`project_id`),
  ADD KEY `administrative_procedure_id` (`administrative_procedure_id`),
  ADD KEY `special_status1_id` (`special_status1_id`),
  ADD KEY `special_status2_id` (`special_status2_id`),
  ADD KEY `special_status3_id` (`special_status3_id`),
  ADD KEY `project_status_id` (`project_status_id`),
  ADD KEY `updated_by` (`updated_by`);

--
-- Index pour la table `project_statuses`
--
ALTER TABLE `project_statuses`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `sectors`
--
ALTER TABLE `sectors`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `special_status1`
--
ALTER TABLE `special_status1`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `special_status2`
--
ALTER TABLE `special_status2`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `special_status3`
--
ALTER TABLE `special_status3`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name` (`name`);

--
-- Index pour la table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `username` (`username`);

--
-- AUTO_INCREMENT pour les tables déchargées
--

--
-- AUTO_INCREMENT pour la table `administrative_procedures`
--
ALTER TABLE `administrative_procedures`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT pour la table `communes`
--
ALTER TABLE `communes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT pour la table `dairas`
--
ALTER TABLE `dairas`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT pour la table `domains`
--
ALTER TABLE `domains`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT pour la table `lots`
--
ALTER TABLE `lots`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT pour la table `programs`
--
ALTER TABLE `programs`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT pour la table `projects`
--
ALTER TABLE `projects`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- AUTO_INCREMENT pour la table `project_statuses`
--
ALTER TABLE `project_statuses`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT pour la table `sectors`
--
ALTER TABLE `sectors`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT pour la table `special_status1`
--
ALTER TABLE `special_status1`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT pour la table `special_status2`
--
ALTER TABLE `special_status2`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT pour la table `special_status3`
--
ALTER TABLE `special_status3`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT pour la table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Contraintes pour les tables déchargées
--

--
-- Contraintes pour la table `communes`
--
ALTER TABLE `communes`
  ADD CONSTRAINT `communes_ibfk_1` FOREIGN KEY (`daira_id`) REFERENCES `dairas` (`id`);

--
-- Contraintes pour la table `lots`
--
ALTER TABLE `lots`
  ADD CONSTRAINT `lots_ibfk_1` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `lots_ibfk_2` FOREIGN KEY (`administrative_procedure_id`) REFERENCES `administrative_procedures` (`id`),
  ADD CONSTRAINT `lots_ibfk_3` FOREIGN KEY (`special_status1_id`) REFERENCES `special_status1` (`id`),
  ADD CONSTRAINT `lots_ibfk_4` FOREIGN KEY (`special_status2_id`) REFERENCES `special_status2` (`id`),
  ADD CONSTRAINT `lots_ibfk_5` FOREIGN KEY (`special_status3_id`) REFERENCES `special_status3` (`id`),
  ADD CONSTRAINT `lots_ibfk_6` FOREIGN KEY (`project_status_id`) REFERENCES `project_statuses` (`id`),
  ADD CONSTRAINT `lots_ibfk_7` FOREIGN KEY (`updated_by`) REFERENCES `users` (`id`);

--
-- Contraintes pour la table `projects`
--
ALTER TABLE `projects`
  ADD CONSTRAINT `projects_ibfk_1` FOREIGN KEY (`program_id`) REFERENCES `programs` (`id`),
  ADD CONSTRAINT `projects_ibfk_2` FOREIGN KEY (`daira_id`) REFERENCES `dairas` (`id`),
  ADD CONSTRAINT `projects_ibfk_3` FOREIGN KEY (`commune_id`) REFERENCES `communes` (`id`),
  ADD CONSTRAINT `projects_ibfk_4` FOREIGN KEY (`domain_id`) REFERENCES `domains` (`id`),
  ADD CONSTRAINT `projects_ibfk_5` FOREIGN KEY (`sector_id`) REFERENCES `sectors` (`id`),
  ADD CONSTRAINT `projects_ibfk_6` FOREIGN KEY (`updated_by`) REFERENCES `users` (`id`);

--
-- Contraintes pour la table `project_details`
--
ALTER TABLE `project_details`
  ADD CONSTRAINT `project_details_ibfk_1` FOREIGN KEY (`project_id`) REFERENCES `projects` (`id`) ON DELETE CASCADE,
  ADD CONSTRAINT `project_details_ibfk_2` FOREIGN KEY (`administrative_procedure_id`) REFERENCES `administrative_procedures` (`id`),
  ADD CONSTRAINT `project_details_ibfk_3` FOREIGN KEY (`special_status1_id`) REFERENCES `special_status1` (`id`),
  ADD CONSTRAINT `project_details_ibfk_4` FOREIGN KEY (`special_status2_id`) REFERENCES `special_status2` (`id`),
  ADD CONSTRAINT `project_details_ibfk_5` FOREIGN KEY (`special_status3_id`) REFERENCES `special_status3` (`id`),
  ADD CONSTRAINT `project_details_ibfk_6` FOREIGN KEY (`project_status_id`) REFERENCES `project_statuses` (`id`),
  ADD CONSTRAINT `project_details_ibfk_7` FOREIGN KEY (`updated_by`) REFERENCES `users` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
