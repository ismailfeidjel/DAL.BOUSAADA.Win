-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Hôte : 127.0.0.1
-- Généré le : jeu. 23 juil. 2026 à 15:01
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
(9, 'فتح'),
(10, 'مؤجلة'),
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
(1, 1, 'بوسعادة'),
(2, 1, 'ولتام'),
(3, 1, 'الهامل'),
(4, 2, 'عين الملح'),
(5, 2, 'سيدي امحمد'),
(6, 2, 'بئر الفضة'),
(7, 2, 'عين فارس'),
(8, 2, 'عين الريش'),
(9, 3, 'بن سرور'),
(10, 3, 'اولاد سليمان'),
(11, 3, 'محمد بوضياف'),
(12, 3, 'الزرزور'),
(13, 4, 'امجدل'),
(14, 4, 'أمناعة'),
(15, 5, 'اولاد سيدي ابراهيم'),
(16, 5, 'بن زوه'),
(17, 6, 'جبل امساعد'),
(18, 6, 'سليم'),
(19, 7, 'الخيانة'),
(20, 7, 'الحوامد'),
(21, 8, 'سيدي عامر'),
(22, 8, 'تامسة');

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
(7, 'الخبانة'),
(4, 'امجدل'),
(5, 'اولاد سيدي ابراهيم'),
(3, 'بن سرور'),
(1, 'بوسعادة'),
(6, 'جبل امساعد'),
(8, 'سيدي عامر'),
(2, 'عين الملح');

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
  `lot_budget` decimal(18,2) DEFAULT 0.00,
  `registered_amount` decimal(18,2) DEFAULT 0.00,
  `consumed_amount` decimal(18,2) DEFAULT 0.00,
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
-- Déchargement des données de la table `lots`
--

INSERT INTO `lots` (`id`, `project_id`, `lot_number`, `lot_name`, `lot_budget`, `registered_amount`, `consumed_amount`, `contractor`, `execution_duration`, `start_date`, `physical_progress`, `administrative_procedure_id`, `special_status1_id`, `special_status2_id`, `special_status3_id`, `project_status_id`, `notes`, `created_at`, `updated_at`, `updated_by`) VALUES
(4, 2, 1, '', 30000000.00, 0.00, 5000000.00, 'مؤسسة البناء الحديثة', 12, NULL, 20.00, 3, 1, 2, 1, 3, 'الأشغال جارية', '2026-06-23 13:36:56', '2026-06-27 00:08:54', NULL),
(5, 3, 1, '', 25000000.00, 0.00, 8000000.00, 'مؤسسة النور', 10, NULL, 35.00, 2, 2, 1, 2, 2, 'الأشغال جارية', '2026-06-23 13:36:56', '2026-07-14 14:31:37', NULL),
(6, 4, 1, '', 40000000.00, 0.00, 0.00, 'مؤسسة الريان', 14, NULL, 45.00, 1, 1, 2, 1, 3, 'نسبة تقدم جيدة', '2026-06-23 13:36:56', '2026-06-27 00:09:02', NULL),
(7, 5, 1, '', 55000000.00, 0.00, 15000000.00, 'مؤسسة بن نس ', 18, NULL, 50.00, 3, 2, 2, 1, 3, 'الأشغال متواصلة', '2026-06-23 13:36:56', '2026-07-13 12:48:30', NULL),
(8, 6, 1, '', 18000000.00, 0.00, 3000000.00, 'مؤسسة الإعمار', 8, NULL, 15.00, 2, 1, 1, 2, 2, 'لم تنطلق الأشغال بعد', '2026-06-23 13:36:56', '2026-06-27 00:09:08', NULL),
(9, 7, 1, '', 70000000.00, 0.00, 0.00, 'مؤسسة الأشغال الكبرى', 16, '2025-01-05', 60.00, 3, 1, 2, 1, 2, 'تقدم ملحوظ', '2026-06-23 13:36:56', '2026-07-22 10:15:52', NULL),
(10, 8, 1, '', 35000000.00, 0.00, 7000000.00, 'مؤسسة البناء والتعمير', 24, '2025-03-15', 30.00, 4, 2, 1, 2, 3, 'في مرحلة الإنجاز', '2026-06-23 13:36:56', '2026-07-23 12:38:33', NULL),
(11, 9, 1, '', 60000000.00, 0.00, 25000000.00, 'مؤسسة الصحة', 20, '2025-01-12', 70.00, 3, 1, 2, 1, 3, 'الأشغال متقدمة', '2026-06-23 13:36:56', '2026-06-26 23:12:49', NULL),
(12, 10, 1, '', 22000000.00, 22000000.00, 4000000.00, 'مؤسسة الحدائق', 6, '2025-04-01', 10.00, 2, 3, 1, 1, 1, 'بداية الأشغال', '2026-06-23 13:36:56', '2026-07-23 11:29:33', NULL),
(13, 11, 1, '', 80000000.00, 45546.00, 0.00, 'xdfxd', 24, NULL, 55.00, 3, 1, 2, 1, 3, 'الأشغال تسير بشكل عادي', '2026-06-23 13:36:56', '2026-07-14 14:31:29', NULL),
(14, 12, 1, '', 5000000.00, 5000000.00, 1200000.00, 'مؤسسة البناء الحديثة', 12, '2026-03-25', 25.00, 5, 3, 2, NULL, 7, 'بن سرور هي بلدية جزائرية تابعة لولاية المسيلة، وتقع في أقصى جنوب الولاية. وهي تعتبر المركز الإداري لـ دائرة بن سرور، وتضم الدائرة إجمالاً 4 بلديات، وهي:', '2026-06-25 11:52:56', '2026-07-23 10:25:58', NULL),
(20, 21, 1, 'حصة A', 5000000.00, 1.00, 0.00, 'مؤسسة البناء الحديثة', NULL, '2026-03-25', 25.00, 5, 3, 2, NULL, 1, 'حصة تجريبية', '2026-06-25 16:07:18', '2026-07-23 10:25:55', NULL),
(21, 21, 2, 'ببب B', 0.00, 1521.00, 0.00, 'مؤسسة البناء الحديثة', NULL, '2026-05-19', 0.00, 5, 2, 2, 2, 3, 'AZAAAZAZAZ', '2026-06-25 16:07:27', '2026-07-23 09:42:03', NULL),
(22, 21, 3, 'احصة C', 0.00, 3.00, 0.00, 'مؤسسة البناء الحديثة', NULL, '2026-06-24', 0.00, 4, 2, 1, 2, 3, 'AZZZZZZZ', '2026-06-25 16:07:31', '2026-07-23 09:42:06', NULL),
(23, 22, 1, 'الحصة رقم 02', 5000000.00, 5000000.00, 0.00, 'مؤسسة البناء الحديثة', 12, '2026-03-26', 25.00, 5, 3, 2, NULL, 1, 'XX', '2026-06-26 07:05:28', '2026-07-23 10:32:19', NULL),
(24, 22, 2, 'حصة رقم 1', 0.00, 0.00, 0.00, 'S', NULL, '2026-07-08', 0.00, 5, 3, 1, 1, 1, 'S', '2026-06-26 07:05:28', '2026-07-23 10:32:21', NULL),
(25, 24, 1, 'الحصة رقم 01', 5000000.00, 5000000.00, 1200000.00, 'مؤسسة البناء الحديثة', 12, '2026-04-19', 25.00, 5, 3, NULL, NULL, 4, 'حصة تجريبية', '2026-07-19 13:09:07', '2026-07-21 14:56:32', NULL);

-- --------------------------------------------------------

--
-- Structure de la table `programs`
--

CREATE TABLE `programs` (
  `id` int(11) NOT NULL,
  `Type` varchar(20) NOT NULL DEFAULT 'ADSEC',
  `Year` int(11) NOT NULL DEFAULT 0,
  `name` varchar(50) NOT NULL,
  `IsClosed` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `programs`
--

INSERT INTO `programs` (`id`, `Type`, `Year`, `name`, `IsClosed`) VALUES
(1, 'ADSEC', 2025, 'ADSEC2025', 0),
(2, 'ADSEC', 2026, 'ADSEC2026', 0),
(3, 'ADSEC', 2027, 'ADSEC2027', 0),
(4, 'CGSCL', 2025, 'CGSCL2025', 0);

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
  `has_lots` tinyint(1) DEFAULT 0,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `updated_at` timestamp NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp(),
  `updated_by` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Déchargement des données de la table `projects`
--

INSERT INTO `projects` (`id`, `operation_number`, `operation_name`, `program_id`, `daira_id`, `commune_id`, `domain_id`, `sector_id`, `has_lots`, `created_at`, `updated_at`, `updated_by`) VALUES
(1, '1', 'تهيئة حضرية بعين فارس +عين العلق -تزفيت +ارصفة', 4, 1, 1, 2, 3, 1, '2026-06-04 12:43:05', '2026-07-21 09:54:03', 1),
(2, '2', 'تهيئة طرقات حي 500 مسكنن', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:05', 1),
(3, '3', 'إنجاز شبكة الإنارة العمومية', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:09', 1),
(4, '4', 'إنجاز شبكة التطهير\r\n ', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:13', 1),
(5, '5', 'تهيئة مدرسة ابتدائية', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:16', 1),
(6, '6', 'إنجاز ملعب جواري\r\n ', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:19', 1),
(7, '7', 'إعادة تأهيل شبكة المياه', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:21', 1),
(8, '8', 'تهيئة مقر بلدية', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:24', 1),
(9, '9', 'إنجاز قاعة علاج', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:27', 1),
(10, '10', 'تهيئة المساحات الخضراء', 2, 1, 1, 1, 1, 0, '2026-06-23 13:32:39', '2026-07-23 11:29:33', 1),
(11, '11', 'إنجاز شبكة الغاز', 2, 1, 1, 2, 1, 0, '2026-06-23 13:32:39', '2026-07-21 09:51:33', 1),
(12, '12', 'إنجاز مدرسة ابتدائيتتت:', 1, 1, 1, 5, 1, 0, '2026-06-25 11:52:56', '2026-07-21 09:51:35', 1),
(21, '13', 'مشروعي', 1, 4, 14, 5, 1, 0, '2026-06-25 16:07:18', '2026-07-23 10:25:55', 1),
(22, '14', 'إنجاز مدرسة ابتداية ...', 1, 7, 20, 5, 1, 1, '2026-06-26 07:05:28', '2026-07-23 10:32:21', 1),
(24, '15', 'إنجاز مدرسة ابتدائية', 4, 7, 14, 5, 1, 0, '2026-07-19 13:09:07', '2026-07-21 14:56:32', 1);

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
  ADD UNIQUE KEY `name` (`name`),
  ADD KEY `idx_type_year` (`Type`,`Year`);

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
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT pour la table `communes`
--
ALTER TABLE `communes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=23;

--
-- AUTO_INCREMENT pour la table `dairas`
--
ALTER TABLE `dairas`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT pour la table `domains`
--
ALTER TABLE `domains`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT pour la table `lots`
--
ALTER TABLE `lots`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=26;

--
-- AUTO_INCREMENT pour la table `programs`
--
ALTER TABLE `programs`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT pour la table `projects`
--
ALTER TABLE `projects`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

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
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
