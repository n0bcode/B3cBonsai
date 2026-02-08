# Technical Specification: 3D/AR, Storage, and PWA Integration

## 1. Executive Summary

This document specifies the technical requirements and architectural changes for B3cBonsai to support advanced product visualization via 3D/AR, a flexible cloud/local storage backend, and a unified Progressive Web App (PWA) experience.

## 2. Technical Requirements

### 2.1 Product 3D/AR Visualization

- **Component:** Google `<model-viewer>`.
- **File Format:** `.glb` (glTF Binary).
- **Constraints:** Maximum file size of **50MB**.
- **Features:** Auto-rotate, AR placement, snapshot capability, and environment lighting selection.

### 2.2 Interactive Hotspot Management

- **Data Structure:** JSON metadata stored in `SanPham.Model3DMetadata`.
- **Schema:**
  ```json
  {
    "hotspots": [{ "position": "x y z", "normal": "nx ny nz", "text": "Label" }]
  }
  ```
- **Staff UI:** A dynamic list editor in the Product Management area to add/remove annotations without manual JSON editing.

### 2.3 Adaptive Storage Backend

- **Providers:**
  - **Local:** File system storage using `IWebHostEnvironment`.
  - **Cloudinary:** Cloud asset management for production.
  - **AWS S3 (Future):** Abstraction layer ready for S3 implementation.
- **Uniform API:** `IFileStorageService` to handle `StoreFileAsync` (single/multiple) and `DeleteFileAsync`.

### 2.4 Progressive Web App (PWA)

- **Scope:** Entire site (Customer & Employee areas).
- **Caching:**
  - **Core:** Home page, CSS, JS.
  - **3D Assets:** Cache-First strategy for `.glb` files to enable offline viewing of previously loaded products.
- **Manifest:** Standalone display mode with `B3cBonsai` branding.

## 3. Architecture & Data Flow

### 3.1 Backend (ASP.NET Core)

- **Controller:** `ManagerProductController` handles file uploads and metadata serialization.
- **Service:** Dependency Injection resolves the storage provider based on `UseCloudinaryStorage` setting.

### 3.2 Frontend

- **Detail Page:** Dynamic script parses JSON metadata to render hotspots on the model.
- **Upsert Page:** JS-based list manager for hotspots.

## 4. Implementation roadmap

1.  **Phase 1: Foundation.** Refactor storage services and unify PWA registration.
2.  **Phase 2: Management.** Implement the dynamic hotspot editor and file size validation.
3.  **Phase 3: User Experience.** Optimize Service Worker caching for large 3D models.
