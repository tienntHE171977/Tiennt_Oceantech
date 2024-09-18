CREATE DATABASE Volunteerisms3;
GO

USE Volunteerisms3;
GO

-- Khi đăng nhập lần đầu sẽ có trạng thái là user, sau đó user có thể đăng kí làm volunteer hoặc làm đại diện các tổ chức hoặc chỉ làm user.
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    role NVARCHAR(50) CHECK (role IN ('admin', 'donor', 'recipient', 'organization'))
);

CREATE TABLE Organizations (
    organization_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(255) NOT NULL,
    representative INT,
    FOREIGN KEY (representative) REFERENCES Users(user_id) ON DELETE SET NULL  
);

CREATE TABLE OrganizationInformation (
    information_id INT IDENTITY(1,1) PRIMARY KEY,
    organization_id INT NOT NULL UNIQUE,
    description  NVARCHAR(MAX) NOT NULL,
    website NVARCHAR(255),
    contact_email NVARCHAR(255),
    phone_number NVARCHAR(20),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (organization_id) REFERENCES Organizations(organization_id) ON DELETE CASCADE 
);
CREATE TABLE OrganizationAdminRequests (
    request_id INT IDENTITY(1,1) PRIMARY KEY,          -- ID của yêu cầu
    user_id INT NOT NULL,                            -- ID của người dùng
    organization_id INT NOT NULL,                    -- ID của tổ chức
    request_status NVARCHAR(50) CHECK (request_status IN ('pending', 'approved', 'rejected')) DEFAULT 'pending',  -- Trạng thái yêu cầu
    request_date DATETIME DEFAULT GETDATE(),          -- Ngày yêu cầu
    approved_by INT NULL,                            -- ID của quản trị viên đã phê duyệt yêu cầu
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE NO ACTION,      -- Khóa ngoại liên kết với Users
    FOREIGN KEY (organization_id) REFERENCES Organizations(organization_id) ON DELETE NO ACTION,  -- Khóa ngoại liên kết với Organizations
    FOREIGN KEY (approved_by) REFERENCES Users(user_id) ON DELETE SET NULL  -- Khóa ngoại liên kết với Users cho quản trị viên phê duyệt
);
CREATE TABLE OrganizationAdmins (
    admin_id INT IDENTITY(1,1) PRIMARY KEY,          -- ID của quản trị viên
    user_id INT NOT NULL,                            -- ID của người dùng
    organization_id INT NOT NULL,                    -- ID của tổ chức mà quản trị viên thuộc về
    position NVARCHAR(100),                          -- Chức vụ của quản trị viên trong tổ chức
    created_at DATETIME DEFAULT GETDATE(),            -- Ngày tạo bản ghi
    added_by INT NULL,                               -- ID của người dùng đã thêm quản trị viên vào tổ chức
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE NO ACTION,      -- Khóa ngoại liên kết với Users
    FOREIGN KEY (organization_id) REFERENCES Organizations(organization_id) ON DELETE NO ACTION,  -- Khóa ngoại liên kết với Organizations
    FOREIGN KEY (added_by) REFERENCES Users(user_id) ON DELETE SET NULL  -- Khóa ngoại liên kết với Users cho người thêm quản trị viên
);
CREATE TABLE Volunteers (
    volunteer_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL UNIQUE,
    volunteer_name NVARCHAR(255),
    phone_number NVARCHAR(20),
    profile_picture NVARCHAR(255),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE  
);
-- Mỗi volunteer chỉ có 1 cv, cv chứa thông tin volunteer, danh sách các skills, các dự án đã tham gia.
CREATE TABLE VolunteerCV (
    cv_id INT IDENTITY(1,1) PRIMARY KEY,
    volunteer_id INT NOT NULL UNIQUE,
    description NVARCHAR(MAX) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (volunteer_id) REFERENCES Volunteers(volunteer_id) ON DELETE CASCADE  
);
-- Mỗi cv có nhiều skills, skills được tạo bởi chính volunteer đó (volunteer 1 n skill)
CREATE TABLE Skills (
    skill_id INT IDENTITY(1,1) PRIMARY KEY,
    skill_name NVARCHAR(255) NOT NULL,
    skill_description NVARCHAR(MAX) NOT NULL,
    volunteer_id INT NOT NULL,
    FOREIGN KEY (volunteer_id) REFERENCES Volunteers(volunteer_id) ON DELETE CASCADE  
);
CREATE TABLE CV_Reviews (
    review_id INT IDENTITY(1,1) PRIMARY KEY,         -- ID của nhận xét
    cv_id INT NOT NULL,                             -- ID của CV được nhận xét
    reviewer_id INT NOT NULL,                       -- ID của người đánh giá
    rating INT CHECK (rating >= 1 AND rating <= 5), -- Đánh giá (số sao) từ 1 đến 5
    comments NVARCHAR(MAX),                         -- Nhận xét về CV
    review_date DATETIME DEFAULT GETDATE(),          -- Ngày nhận xét
    FOREIGN KEY (cv_id) REFERENCES VolunteerCV(cv_id) ON DELETE CASCADE,  
    FOREIGN KEY (reviewer_id) REFERENCES Users(user_id) 
);

-- Team được tạo bởi 1 volunteer(leader)
CREATE TABLE Teams (
    team_id INT PRIMARY KEY IDENTITY(1,1),
    team_name NVARCHAR(100) NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    created_by INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    is_active BIT DEFAULT 1,
    FOREIGN KEY (created_by) REFERENCES Users(user_id) 
);
-- Danh sách các thành viên từng team
CREATE TABLE TeamMembers (
    team_member_id INT PRIMARY KEY IDENTITY(1,1),
    team_id INT NOT NULL,
    user_id INT NOT NULL,
    joined_date DATETIME DEFAULT GETDATE(),
    RoleInTeam NVARCHAR(50) CHECK (RoleInTeam IN ('Member', 'Leader')) DEFAULT 'Member',
    FOREIGN KEY (team_id) REFERENCES Teams(team_id) ON DELETE CASCADE,  
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE  
);
-- TeamReport cho phép user báo cáo team khi có sai phạm
CREATE TABLE TeamReports (
    report_id INT IDENTITY(1,1) PRIMARY KEY,       
    reported_by INT NOT NULL,                       
    team_id INT NOT NULL,                           
    report_reason NVARCHAR(MAX) NOT NULL,           
    report_date DATETIME DEFAULT GETDATE(),          
    status NVARCHAR(50) CHECK (status IN ('Pending', 'Reviewed', 'Resolved')) DEFAULT 'Pending',
    FOREIGN KEY (reported_by) REFERENCES Users(user_id) ,  
    FOREIGN KEY (team_id) REFERENCES Teams(team_id) ON DELETE CASCADE          
);
-- địa điểm để tổ chức Campaigns
CREATE TABLE Locations (
    location_id INT PRIMARY KEY IDENTITY(1,1),       
    address NVARCHAR(MAX) NOT NULL, 
    city NVARCHAR(100) NOT NULL, 
	province NVARCHAR(100),                      
    country NVARCHAR(100) NOT NULL,             
    created_at DATETIME DEFAULT GETDATE()   
);
-- Các danh mục dùng chung cho cả Campaigns và Project (Ex: xã hội, người già, trẻ nhỏ,...)
CREATE TABLE VCategories (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(255) NOT NULL,       
    description NVARCHAR(MAX)                   
);
-- Campaign là các hoạt động thiện nguyện được tổ chức cụ thể (Ex: trồng cây xanh tại xã..huyện..)
CREATE TABLE Campaigns (
    campaign_id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    campaign_name NVARCHAR(200) NOT NULL,
	location_id INT NULL,
    description NVARCHAR(MAX),
    startDate DATE,
    endDate DATE,
    is_active BIT DEFAULT 1,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES Users(user_id),
	FOREIGN KEY (location_id) REFERENCES Locations(location_id)
);
CREATE TABLE Jobs (
    job_id INT IDENTITY(1,1) PRIMARY KEY,        
    title NVARCHAR(255) NOT NULL,                -- Tiêu đề công việc
    description NVARCHAR(MAX) NOT NULL,          -- Mô tả công việc
    requirements NVARCHAR(MAX),                  -- Các yêu cầu của công việc
    start_date DATE,                             -- Ngày bắt đầu công việc
    end_date DATE,                               -- Ngày kết thúc công việc
    location NVARCHAR(255),                      -- Địa điểm thực hiện công việc
    created_at DATETIME DEFAULT GETDATE(),        -- Ngày tạo công việc
    campaign_id INT,                             -- ID của chiến dịch mà công việc thuộc về
    FOREIGN KEY (campaign_id) REFERENCES Campaigns(campaign_id) ON DELETE SET NULL  -- Ràng buộc khóa ngoại với Campaigns
);

-- Bảng dưới chứa thông tin của những người, volunteer, team quan tâm đến Campaign hoặc tham gia
CREATE TABLE CampaignParticipants (
    participant_id INT PRIMARY KEY IDENTITY(1,1),
    campaign_id INT NOT NULL,
    volunteer_id INT NULL,  
    team_id INT NULL,       
    participation_type NVARCHAR(50) CHECK (participation_type IN ('Interested', 'Joined')),
    participation_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (campaign_id) REFERENCES Campaigns(campaign_id) ON DELETE CASCADE,
    FOREIGN KEY (volunteer_id) REFERENCES Volunteers(volunteer_id) ON DELETE CASCADE,
    FOREIGN KEY (team_id) REFERENCES Teams(team_id) ON DELETE CASCADE
);
-- Mỗi mội người tham gia campaign (trạng thái joined ở bảng trên) phải nhận 1 job của campaign đó
CREATE TABLE Job_Volunteers (
    job_volunteer_id INT IDENTITY(1,1) PRIMARY KEY,  
    job_id INT NOT NULL,                             -- ID của công việc
    volunteer_id INT NOT NULL,                       -- ID của tình nguyện viên
    status NVARCHAR(50) CHECK (status IN ('applied', 'accepted', 'completed', 'rejected')) DEFAULT 'applied', -- Trạng thái tình nguyện viên trong công việc
    assigned_date DATETIME DEFAULT GETDATE(),        -- Ngày phân công
    FOREIGN KEY (job_id) REFERENCES Jobs(job_id) ON DELETE CASCADE,  -- Ràng buộc khóa ngoại với Jobs
    FOREIGN KEY (volunteer_id) REFERENCES Volunteers(volunteer_id) ON DELETE CASCADE  -- Ràng buộc khóa ngoại với Volunteers
);
CREATE TABLE CampaignCategories (
    campaign_id INT NOT NULL,
    category_id INT NOT NULL,
    PRIMARY KEY (campaign_id, category_id),     
    FOREIGN KEY (campaign_id) REFERENCES Campaigns(campaign_id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES VCategories(category_id) ON DELETE CASCADE
);
-- Dùng để chức các bình luận của user đối với campaign
CREATE TABLE CampaignReviews (
    review_id INT IDENTITY(1,1) PRIMARY KEY, 
    user_id INT NOT NULL,  
    campaign_id INT NOT NULL,  
    rating INT CHECK (rating >= 1 AND rating <= 5),  -- Đánh giá từ 1 đến 5 sao
    comment NVARCHAR(MAX),  
    created_at DATETIME DEFAULT GETDATE(),  
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE,  
    FOREIGN KEY (campaign_id) REFERENCES Campaigns(campaign_id) ON DELETE CASCADE 
);
-- Project đây là các dự án kêu gọi quyên góp(ở đây là nhu yếu phẩm)
CREATE TABLE Projects (
    project_id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    start_date DATE,
    end_date DATE,
    status NVARCHAR(50) CHECK (status IN ('pending', 'approved', 'rejected')) DEFAULT 'pending', -- Trạng thái kiểm duyệt
    -- Người tạo có thể là User hoặc Organization
    created_by_user INT NULL,  -- Nếu người tạo là User
    created_by_org INT NULL,   -- Nếu người tạo là Organization
 
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (created_by_user) REFERENCES Users(user_id) ON DELETE SET NULL,
    FOREIGN KEY (created_by_org) REFERENCES Organizations(organization_id) ON DELETE SET NULL
);
-- Chứa thông tin người quanr lý dự án
CREATE TABLE Project_Management (
    management_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,  -- Người quản lý phải là một User
    project_id INT NOT NULL,  -- Dự án mà người này quản lý
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ,
    FOREIGN KEY (project_id) REFERENCES Projects(project_id) 
);
-- Kiểm duyệt dự án
CREATE TABLE Project_Censorship (
    censorship_id INT IDENTITY(1,1) PRIMARY KEY,
    project_id INT NOT NULL,              
    admin_id INT NOT NULL,                -- ID của người kiểm duyệt (admin)
    status NVARCHAR(50) CHECK (status IN ('approved', 'rejected')) NOT NULL,  -- Trạng thái kiểm duyệt
    comments NVARCHAR(MAX) NOT NULL,      -- Lời nhận xét của admin
    censorship_date DATETIME DEFAULT GETDATE(),  
    is_latest BIT DEFAULT 1,              
    FOREIGN KEY (project_id) REFERENCES Projects(project_id) ,  
    FOREIGN KEY (admin_id) REFERENCES Users(user_id)          
);
-- Danh sách các nhu yếu phẩm cần quyên góp
CREATE TABLE Supplies (
    supply_id INT IDENTITY(1,1) PRIMARY KEY,
    project_id INT NOT NULL,                         
    item_name NVARCHAR(255) NOT NULL,                
    quantity_needed INT NOT NULL,                    
    quantity_provided INT DEFAULT 0,                 
    unit NVARCHAR(50),                               
    FOREIGN KEY (project_id) REFERENCES Projects(project_id) 
);
CREATE TABLE Project_Documents (
    document_id INT IDENTITY(1,1) PRIMARY KEY,         
    project_id INT NOT NULL,                           -- ID của dự án liên quan
    document_name NVARCHAR(255) NOT NULL,              -- Tên tài liệu
    document_type NVARCHAR(50) NOT NULL,               -- Loại tài liệu (ví dụ: 'Report', 'Plan', 'Proposal')
    document_url NVARCHAR(MAX) NOT NULL,               -- URL hoặc đường dẫn đến tài liệu
    created_at DATETIME DEFAULT GETDATE(),              -- Ngày tạo tài liệu
    FOREIGN KEY (project_id) REFERENCES Projects(project_id)  
);
CREATE TABLE ProjectCategories (
    project_id INT NOT NULL,
    category_id INT NOT NULL,
    PRIMARY KEY (project_id, category_id),  
    FOREIGN KEY (project_id) REFERENCES Projects(project_id) ,  
    FOREIGN KEY (category_id) REFERENCES VCategories(category_id) 
);
CREATE TABLE ProjectReviews (
    review_id INT IDENTITY(1,1) PRIMARY KEY,  
    user_id INT NOT NULL,  
    project_id INT NOT NULL,  
    rating INT CHECK (rating >= 1 AND rating <= 5),  -- Đánh giá từ 1 đến 5 sao
    comment NVARCHAR(MAX),  
    created_at DATETIME DEFAULT GETDATE(), 
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ,
    FOREIGN KEY (project_id) REFERENCES Projects(project_id) 
);
-- Danh sách các nhà tài trợ
CREATE TABLE Donors (
    donor_id INT IDENTITY(1,1) PRIMARY KEY,       
    user_id INT NOT NULL UNIQUE,                  
    donor_name NVARCHAR(255) NOT NULL,           
    email NVARCHAR(255) NOT NULL,                
    phone_number NVARCHAR(20),                    
    created_at DATETIME DEFAULT GETDATE(),        
    FOREIGN KEY (user_id) REFERENCES Users(user_id) 
);
-- Danh sách các dự án mà nhà tài trợ đã tham gia đóng góp
CREATE TABLE Donations (
    donation_id INT IDENTITY(1,1) PRIMARY KEY,    
    donor_id INT NOT NULL,                        
    project_id INT NOT NULL,                      
    donation_level INT NOT NULL,      -- Mức độ đóng góp cho project
    donation_date DATETIME DEFAULT GETDATE(),     
    FOREIGN KEY (donor_id) REFERENCES Donors(donor_id) ,
    FOREIGN KEY (project_id) REFERENCES Projects(project_id) 
);
-- Vinh danh nhà tài trợ
CREATE TABLE DonorRecognition (
    recognition_id INT IDENTITY(1,1) PRIMARY KEY,    -- ID của bảng vinh danh
    donor_id INT NOT NULL,                           -- ID của nhà tài trợ
    total_projects INT DEFAULT 0,                    -- Tổng số dự án mà nhà tài trợ đã tham gia
    recognition_level NVARCHAR(50) CHECK (recognition_level IN ('Bronze', 'Silver', 'Gold', 'Platinum')) DEFAULT 'Bronze',  -- Mức độ vinh danh
    FOREIGN KEY (donor_id) REFERENCES Donors(donor_id)  
);
CREATE TABLE Notification (
    notification_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    message  NVARCHAR(MAX) NOT NULL,
    is_read BIT DEFAULT 0,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES Users(user_id)
);


-- Table Reports dùng để báo cáo danh sách các project (dùng để xuất file text,word...)
CREATE TABLE Reports (
    report_id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(255) NOT NULL,
    content  NVARCHAR(MAX) NOT NULL,
    created_by INT NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (created_by) REFERENCES Users(user_id)
);

CREATE TABLE User_Support (
    support_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    subject NVARCHAR(255) NOT NULL,
    description  NVARCHAR(MAX) NOT NULL,
    status NVARCHAR(50) CHECK (status IN ('open', 'closed')) DEFAULT 'open',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES Users(user_id)
);

CREATE TABLE System_Settings (
    setting_id INT IDENTITY(1,1) PRIMARY KEY,
    setting_name NVARCHAR(255) NOT NULL,
    setting_value  NVARCHAR(MAX) NOT NULL,
    updated_at DATETIME DEFAULT GETDATE()
);
CREATE TABLE NewsCategories (
    category_id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(100) NOT NULL
);
CREATE TABLE News (
    news_id INT IDENTITY(1,1) PRIMARY KEY,
    title NVARCHAR(255) NOT NULL,
	category_id INT NOT NULL,
    content  NVARCHAR(MAX) NOT NULL,
    author_id INT,
    publish_date DATETIME DEFAULT GETDATE(),
    status NVARCHAR(50),
    created_at DATETIME DEFAULT GETDATE(),
    updated_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (author_id) REFERENCES Users(user_id),
	FOREIGN KEY (category_id) REFERENCES NewsCategories(category_id)
);