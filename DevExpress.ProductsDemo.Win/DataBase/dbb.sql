-- phpMyAdmin SQL Dump
-- version 5.1.3
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Apr 18, 2026 at 11:19 PM
-- Server version: 10.4.24-MariaDB
-- PHP Version: 7.4.29

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `dbb`
--

-- --------------------------------------------------------

--
-- Table structure for table `adsec_projects`
--

CREATE TABLE `adsec_projects` (
  `id` int(11) NOT NULL,
  `comune_id` int(11) NOT NULL,
  `intitule_pri` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `programme_year` smallint(6) NOT NULL,
  `field_name` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `sector_name` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `registration_montat` decimal(14,2) NOT NULL DEFAULT 0.00,
  `financial_consumption` decimal(14,2) NOT NULL DEFAULT 0.00,
  `financial_progress` decimal(5,2) NOT NULL DEFAULT 0.00,
  `project_status` varchar(50) COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `adsec_projects`
--

INSERT INTO `adsec_projects` (`id`, `comune_id`, `intitule_pri`, `programme_year`, `field_name`, `sector_name`, `registration_montat`, `financial_consumption`, `financial_progress`, `project_status`, `created_at`, `updated_at`) VALUES
(1, 13, 'مشروع 1', 2026, 'مياه', 'طاقة', '0.00', '0.00', '0.00', 'جارية', '2026-04-18 19:41:56', '2026-04-18 19:41:56'),
(2, 13, 'مشروع 2', 2026, 'غبغ', 'نهت', '0.00', '0.00', '0.00', 'جارية', '2026-04-18 19:41:56', '2026-04-18 19:41:56');

-- --------------------------------------------------------

--
-- Table structure for table `adsec_project_tasks`
--

CREATE TABLE `adsec_project_tasks` (
  `id` int(11) NOT NULL,
  `parent_id` int(11) NOT NULL,
  `task_title` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `financial_montont_pre` decimal(14,2) NOT NULL DEFAULT 0.00,
  `financial_remaining` decimal(14,2) NOT NULL DEFAULT 0.00,
  `contructor` varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `ods_date` date DEFAULT NULL,
  `pysical_progress` decimal(5,2) NOT NULL DEFAULT 0.00,
  `notes` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT current_timestamp(),
  `updated_at` datetime NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `adsec_project_tasks`
--

INSERT INTO `adsec_project_tasks` (`id`, `parent_id`, `task_title`, `financial_montont_pre`, `financial_remaining`, `contructor`, `duration`, `ods_date`, `pysical_progress`, `notes`, `created_at`, `updated_at`) VALUES
(1, 1, '', '0.00', '0.00', 'علي', 123, '2026-04-15', '0.00', 'بلا بلا بلا', '2026-04-18 20:25:12', '2026-04-18 20:25:12'),
(2, 2, 'يبيب', '0.00', '0.00', 'محمج', 30, '2026-04-22', '0.00', 'بلابلهتليهلس', '2026-04-18 20:25:12', '2026-04-18 20:25:12'),
(3, 2, 'بالبال', '0.00', '0.00', NULL, 123, NULL, '0.00', 'صمنثبصبنصثبنصمنب', '2026-04-18 20:25:57', '2026-04-18 20:25:57'),
(4, 2, 'kdkdkkdk', '0.00', '0.00', 'sdk', 12, NULL, '0.00', 'SIODIJQIJ', '2026-04-18 21:26:47', '2026-04-18 21:26:47');

-- --------------------------------------------------------

--
-- Table structure for table `comunes`
--

CREATE TABLE `comunes` (
  `id` int(11) NOT NULL,
  `iddaira` int(11) NOT NULL,
  `name` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

--
-- Dumping data for table `comunes`
--

INSERT INTO `comunes` (`id`, `iddaira`, `name`) VALUES
(1, 1, 'بوسعادة'),
(2, 1, 'الهامل'),
(3, 1, 'ولتام'),
(4, 3, 'عين الملح'),
(5, 3, 'عين الريش'),
(6, 3, 'بئر فضة'),
(7, 3, 'عين فارس'),
(8, 3, 'سيدي امحمد'),
(9, 6, 'بن سرور'),
(10, 6, 'محمد بوضياف'),
(11, 6, 'اولاد سليمان'),
(12, 6, 'الزرزور'),
(13, 7, 'الخبانة'),
(14, 7, 'مسيف'),
(15, 7, 'الحوامد'),
(16, 4, 'سيدي عامر'),
(17, 4, 'تامسة'),
(18, 8, 'امجدل'),
(19, 8, 'مناعة'),
(20, 5, 'جبل امساعد'),
(21, 5, 'سليم'),
(22, 2, 'اولاد سيدي ابراهيم'),
(23, 2, 'بن زوه');

-- --------------------------------------------------------

--
-- Table structure for table `daira`
--

CREATE TABLE `daira` (
  `id` int(11) NOT NULL,
  `name` text NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

--
-- Dumping data for table `daira`
--

INSERT INTO `daira` (`id`, `name`) VALUES
(1, 'بوسعادة'),
(2, 'أولاد سيدي إبراهيم'),
(3, 'عين الملح'),
(4, 'سيدي عامر'),
(5, 'جبل امساعد'),
(6, 'بن سرور'),
(7, 'خبانة'),
(8, 'امجدل');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `adsec_projects`
--
ALTER TABLE `adsec_projects`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ix_adsec_projects_comune_id` (`comune_id`),
  ADD KEY `ix_adsec_projects_project_status` (`project_status`);

--
-- Indexes for table `adsec_project_tasks`
--
ALTER TABLE `adsec_project_tasks`
  ADD PRIMARY KEY (`id`),
  ADD KEY `ix_tasks_parent_id` (`parent_id`);

--
-- Indexes for table `comunes`
--
ALTER TABLE `comunes`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `ux_comunes_name` (`name`),
  ADD KEY `iddaira` (`iddaira`);

--
-- Indexes for table `daira`
--
ALTER TABLE `daira`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `adsec_projects`
--
ALTER TABLE `adsec_projects`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `adsec_project_tasks`
--
ALTER TABLE `adsec_project_tasks`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `comunes`
--
ALTER TABLE `comunes`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=24;

--
-- AUTO_INCREMENT for table `daira`
--
ALTER TABLE `daira`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `adsec_projects`
--
ALTER TABLE `adsec_projects`
  ADD CONSTRAINT `fk_adsec_projects_comune` FOREIGN KEY (`comune_id`) REFERENCES `comunes` (`id`) ON UPDATE CASCADE;

--
-- Constraints for table `adsec_project_tasks`
--
ALTER TABLE `adsec_project_tasks`
  ADD CONSTRAINT `fk_tasks_project` FOREIGN KEY (`parent_id`) REFERENCES `adsec_projects` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;

--
-- Constraints for table `comunes`
--
ALTER TABLE `comunes`
  ADD CONSTRAINT `comunes_ibfk_1` FOREIGN KEY (`iddaira`) REFERENCES `daira` (`id`) ON DELETE CASCADE ON UPDATE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
