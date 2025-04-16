create database Oceantech;
use Oceantech

-- Tạo bảng DanhMucTinh
CREATE TABLE DanhMucTinh (
    TinhID INT PRIMARY KEY IDENTITY(1,1),
    TenTinh NVARCHAR(100) NOT NULL
);

-- Tạo bảng DanhMucHuyen
CREATE TABLE DanhMucHuyen (
    HuyenID INT PRIMARY KEY IDENTITY(1,1),
    TenHuyen NVARCHAR(100) NOT NULL,
    TinhID INT FOREIGN KEY REFERENCES DanhMucTinh(TinhID)
);

-- Tạo bảng DanhMucXa
CREATE TABLE DanhMucXa (
    XaID INT PRIMARY KEY IDENTITY(1,1),
    TenXa NVARCHAR(100) NOT NULL,
    HuyenID INT FOREIGN KEY REFERENCES DanhMucHuyen(HuyenID)
);

-- Tạo bảng DanToc
CREATE TABLE DanToc (
    DanTocID INT PRIMARY KEY IDENTITY(1,1),
    TenDanToc NVARCHAR(100) NOT NULL
);

-- Tạo bảng NgheNghiep
CREATE TABLE NgheNghiep (
    NgheNghiepID INT PRIMARY KEY IDENTITY(1,1),
    TenNgheNghiep NVARCHAR(100) NOT NULL
);

-- Tạo bảng Employee
CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    Tuoi INT,
    DanTocID INT FOREIGN KEY REFERENCES DanToc(DanTocID),
    NgheNghiepID INT FOREIGN KEY REFERENCES NgheNghiep(NgheNghiepID),
    CCCD NVARCHAR(20),
    SoDienThoai NVARCHAR(15),
    TinhID INT FOREIGN KEY REFERENCES DanhMucTinh(TinhID),
    HuyenID INT FOREIGN KEY REFERENCES DanhMucHuyen(HuyenID),
    XaID INT FOREIGN KEY REFERENCES DanhMucXa(XaID),
    DiaChiCuThe NVARCHAR(255)
);
CREATE TABLE VanBang (
    VanBangID INT PRIMARY KEY IDENTITY(1,1),
    TenVanBang NVARCHAR(100) NOT NULL,
    NgayCap DATE NOT NULL,
    DonViCap INT FOREIGN KEY REFERENCES DanhMucTinh(TinhID),
    NgayHetHan DATE,
    EmployeeID INT FOREIGN KEY REFERENCES Employee(EmployeeID)
);
ALTER TABLE VanBang
ADD CONSTRAINT CK_VanBang_Max3VanBangChuaHetHan
CHECK (
    (SELECT COUNT(*) FROM VanBang vb WHERE vb.EmployeeID = EmployeeID AND (vb.NgayHetHan IS NULL OR vb.NgayHetHan > GETDATE())) <= 3
);
-- Chèn dữ liệu vào bảng Employee
INSERT INTO Employee (HoTen, NgaySinh, Tuoi, DanTocID, NgheNghiepID, CCCD, SoDienThoai, TinhID, HuyenID, XaID, DiaChiCuThe)
VALUES 
('Nguyễn Văn Tien', '2003-01-01', 22, 3, 4, NULL, NULL, 1, 1, 1, 'HP'),
('Trần Thị B', '1985-05-15', 38, 2, 2, '234567890123', '0912345678', 2, 4, 7, 'Số 2, Bến Nghé, Quận 1, Hồ Chí Minh'),
('Lê Văn C', '1992-07-20', 31, 3, 3, '345678901234', '0923456789', 3, 7, 8, 'Số 3, Hải Châu, Đà Nẵng'),
('Phạm Thị D', '1988-03-25', 35, 4, 4, '456789012345', '0934567890', 4, 9, 9, 'Số 4, Ngô Quyền, Hải Phòng'),
('Hoàng Văn E', '1995-11-30', 28, 5, 5, '567890123456', '0945678901', 5, 10, 10, 'Số 5, Hồng Bàng, Cần Thơ'),
('Vũ Thị F', '1991-09-12', 32, 6, 6, '678901234567', '0956789012', 6, 6, 6, 'Số 6, Quận 5, Hồ Chí Minh'),
('Bùi Thị H', '1993-12-05', 30, 8, 8, '890123456789', '0978901234', 8, 9, 9, 'Số 8, Ngô Quyền, Hải Phòng'),
('Nguyễn Văn I', '1994-08-22', 29, 9, 9, '901234567890', '0989012345', 9, 10, 10, 'Số 9, Hồng Bàng, Cần Thơ'),
('Trần Thị K', '1996-06-10', 27, 10, 10, '012345678901', '0990123456', 10, 1, 1, 'Số 10, Phúc Xá, Ba Đình, Hà Nội'),
('Nguyễn Trung Tiến', '2003-01-05', 22, 4, 9, '999999999999', NULL, 1, 1, 1, 'HP'),
('Tiến NT', '2003-02-02', 22, 3, 3, NULL, NULL, 2, 5, 9, 'HP'),
('Nguyễn Trung Tiến', '2007-02-21', 18, 2, 1, '000000000000', NULL, 1, 1, 1, 'HP');

-- Chèn dữ liệu vào bảng VanBang
-- Đảm bảo mỗi nhân viên chỉ có tối đa 3 văn bằng chưa hết hạn
-- Chèn dữ liệu vào bảng Employee
INSERT INTO Employee (HoTen, NgaySinh, Tuoi, DanTocID, NgheNghiepID, CCCD, SoDienThoai, TinhID, HuyenID, XaID, DiaChiCuThe)
VALUES 
('Nguyễn Văn Tien', '2003-01-01', 22, 3, 4, NULL, NULL, 1, 1, 1, 'HP'),
('Trần Thị B', '1985-05-15', 38, 2, 2, '234567890123', '0912345678', 2, 4, 7, 'Số 2, Bến Nghé, Quận 1, Hồ Chí Minh'),
('Lê Văn C', '1992-07-20', 31, 3, 3, '345678901234', '0923456789', 3, 7, 8, 'Số 3, Hải Châu, Đà Nẵng'),
('Phạm Thị D', '1988-03-25', 35, 4, 4, '456789012345', '0934567890', 4, 9, 9, 'Số 4, Ngô Quyền, Hải Phòng'),
('Hoàng Văn E', '1995-11-30', 28, 5, 5, '567890123456', '0945678901', 5, 10, 10, 'Số 5, Hồng Bàng, Cần Thơ'),
('Vũ Thị F', '1991-09-12', 32, 6, 6, '678901234567', '0956789012', 6, 6, 6, 'Số 6, Quận 5, Hồ Chí Minh'),
('Bùi Thị H', '1993-12-05', 30, 8, 8, '890123456789', '0978901234', 8, 9, 9, 'Số 8, Ngô Quyền, Hải Phòng'),
('Nguyễn Văn I', '1994-08-22', 29, 9, 9, '901234567890', '0989012345', 9, 10, 10, 'Số 9, Hồng Bàng, Cần Thơ'),
('Trần Thị K', '1996-06-10', 27, 10, 10, '012345678901', '0990123456', 10, 1, 1, 'Số 10, Phúc Xá, Ba Đình, Hà Nội'),
('Nguyễn Trung Tiến', '2003-01-05', 22, 4, 9, '999999999999', NULL, 1, 1, 1, 'HP'),
('Tiến NT', '2003-02-02', 22, 3, 3, NULL, NULL, 2, 5, 9, 'HP'),
('Nguyễn Trung Tiến', '2007-02-21', 18, 2, 1, '000000000000', NULL, 1, 1, 1, 'HP');

-- Chèn dữ liệu vào bảng VanBang
-- Đảm bảo mỗi nhân viên chỉ có tối đa 3 văn bằng chưa hết hạn
INSERT INTO VanBang (TenVanBang, NgayCap, DonViCap, NgayHetHan, EmployeeID)
VALUES 
-- Văn bằng chưa hết hạn
('Văn bằng 1', '2022-01-01', 1, '2025-01-01', 1),
('Văn bằng 2', '2022-02-01', 1, '2025-02-01', 1),
('Văn bằng 3', '2022-03-01', 1, '2025-03-01', 1),
('Văn bằng 4', '2022-04-01', 2, '2025-04-01', 2),
('Văn bằng 5', '2022-05-01', 2, '2025-05-01', 2),
('Văn bằng 6', '2022-06-01', 2, '2025-06-01', 2),
('Văn bằng 7', '2022-07-01', 3, '2025-07-01', 3),
('Văn bằng 8', '2022-08-01', 3, '2025-08-01', 3),
('Văn bằng 9', '2022-09-01', 3, '2025-09-01', 3),
('Văn bằng 10', '2022-10-01', 4, '2025-10-01', 4),
('Văn bằng 11', '2022-11-01', 4, '2025-11-01', 4),
('Văn bằng 12', '2022-12-01', 4, '2025-12-01', 4),
('Văn bằng 13', '2023-01-01', 5, '2026-01-01', 5),
('Văn bằng 14', '2023-02-01', 5, '2026-02-01', 5),
('Văn bằng 15', '2023-03-01', 5, '2026-03-01', 5),
('Văn bằng 16', '2023-04-01', 6, '2026-04-01', 6),
('Văn bằng 17', '2023-05-01', 6, '2026-05-01', 6),
('Văn bằng 18', '2023-06-01', 6, '2026-06-01', 6),
('Văn bằng 19', '2023-07-01', 8, '2026-07-01', 8),
('Văn bằng 20', '2023-08-01', 8, '2026-08-01', 8),
('Văn bằng 21', '2023-09-01', 8, '2026-09-01', 8),
('Văn bằng 22', '2023-10-01', 9, '2026-10-01', 9),
('Văn bằng 23', '2023-11-01', 9, '2026-11-01', 9),
('Văn bằng 24', '2023-12-01', 9, '2026-12-01', 9),
('Văn bằng 25', '2024-01-01', 10, '2027-01-01', 10),
('Văn bằng 26', '2024-02-01', 10, '2027-02-01', 10),
('Văn bằng 27', '2024-03-01', 10, '2027-03-01', 10),
('Văn bằng 28', '2024-04-01', 1, '2027-04-01', 12),
('Văn bằng 29', '2024-05-01', 1, '2027-05-01', 12),
('Văn bằng 30', '2024-06-01', 1, '2027-06-01', 12),
('Văn bằng 31', '2024-07-01', 2, '2027-07-01', 13),
('Văn bằng 32', '2024-08-01', 2, '2027-08-01', 13),
('Văn bằng 33', '2024-09-01', 2, '2027-09-01', 13),
('Văn bằng 34', '2024-10-01', 4, '2027-10-01', 14),
('Văn bằng 35', '2024-11-01', 4, '2027-11-01', 14),
('Văn bằng 36', '2024-12-01', 4, '2027-12-01', 14),

-- Văn bằng đã hết hạn
('Văn bằng 37', '2019-01-01', 1, '2022-01-01', 1),
('Văn bằng 38', '2019-02-01', 1, '2022-02-01', 1),
('Văn bằng 39', '2019-03-01', 1, '2022-03-01', 1),
('Văn bằng 40', '2019-04-01', 2, '2022-04-01', 2),
('Văn bằng 41', '2019-05-01', 2, '2022-05-01', 2),
('Văn bằng 42', '2019-06-01', 2, '2022-06-01', 2),
('Văn bằng 43', '2019-07-01', 3, '2022-07-01', 3),
('Văn bằng 44', '2019-08-01', 3, '2022-08-01', 3),
('Văn bằng 45', '2019-09-01', 3, '2022-09-01', 3),
('Văn bằng 46', '2019-10-01', 4, '2022-10-01', 4),
('Văn bằng 47', '2019-11-01', 4, '2022-11-01', 4),
('Văn bằng 48', '2019-12-01', 4, '2022-12-01', 4),
('Văn bằng 49', '2020-01-01', 5, '2023-01-01', 5),
('Văn bằng 50', '2020-02-01', 5, '2023-02-01', 5),
('Văn bằng 51', '2020-03-01', 5, '2023-03-01', 5),
('Văn bằng 52', '2020-04-01', 6, '2023-04-01', 6),
('Văn bằng 53', '2020-05-01', 6, '2023-05-01', 6),
('Văn bằng 54', '2020-06-01', 6, '2023-06-01', 6),
('Văn bằng 55', '2020-07-01', 8, '2023-07-01', 8),
('Văn bằng 56', '2020-08-01', 8, '2023-08-01', 8),
('Văn bằng 57', '2020-09-01', 8, '2023-09-01', 8),
('Văn bằng 58', '2020-10-01', 9, '2023-10-01', 9),
('Văn bằng 59', '2020-11-01', 9, '2023-11-01', 9),
('Văn bằng 60', '2020-12-01', 9, '2023-12-01', 9),
('Văn bằng 61', '2021-01-01', 10, '2024-01-01', 10),
('Văn bằng 62', '2021-02-01', 10, '2024-02-01', 10),
('Văn bằng 63', '2021-03-01', 10, '2024-03-01', 10),
('Văn bằng 64', '2021-04-01', 1, '2024-04-01', 12),
('Văn bằng 65', '2021-05-01', 1, '2024-05-01', 12),
('Văn bằng 66', '2021-06-01', 1, '2024-06-01', 12),
('Văn bằng 67', '2021-07-01', 2, '2024-07-01', 13),
('Văn bằng 68', '2021-08-01', 2, '2024-08-01', 13),
('Văn bằng 69', '2021-09-01', 2, '2024-09-01', 13),
('Văn bằng 70', '2021-10-01', 4, '2024-10-01', 14),
('Văn bằng 71', '2021-11-01', 4, '2024-11-01', 14),
('Văn bằng 72', '2021-12-01', 4, '2024-12-01', 14);

