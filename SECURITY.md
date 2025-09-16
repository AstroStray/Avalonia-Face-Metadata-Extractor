# Security Policy

## Supported Versions

We actively support the following versions of MetaExtractor with security updates:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |
| < 1.0   | :x:                |

## Reporting a Vulnerability

### How to Report

If you discover a security vulnerability within MetaExtractor, please send an e-mail to the maintainers. **Do not** create a public GitHub issue for security vulnerabilities.

**Contact:** Create a private security advisory through GitHub's security tab or contact the repository owner directly.

### What to Include

When reporting a vulnerability, please include:

1. **Description** of the vulnerability
2. **Steps to reproduce** the issue
3. **Potential impact** assessment
4. **Suggested fix** (if you have one)
5. **Your contact information** for follow-up

### Response Timeline

- **Acknowledgment:** Within 48 hours
- **Initial Assessment:** Within 1 week  
- **Status Update:** Every week until resolved
- **Resolution:** Depends on severity and complexity

### Security Best Practices

This project follows these security practices:

#### Dependencies
- Regular dependency updates via Dependabot
- Automated security vulnerability scanning
- Review of all dependency updates before merging

#### Code Security
- No hardcoded secrets or API keys
- Input validation for all external data
- Secure handling of image/video files
- Memory management for OpenCV operations

#### Data Privacy
- Face detection data is processed locally
- No automatic data transmission to external services
- User controls over data retention and deletion
- Anonymization options for exported data

#### Build & Deployment
- Secure CI/CD pipeline with vulnerability scanning
- Code signing for releases (planned)
- Container security best practices (if applicable)

### Known Security Considerations

#### OpenCV Dependencies
- Uses OpenCV for computer vision operations
- Keep OpenCV libraries updated for security patches
- Validate input image/video formats

#### Model Files
- ONNX models should be verified and trusted sources only
- Implement model validation before loading
- Sandboxed model execution environment

#### User Data
- Face analysis results may contain sensitive biometric data
- Follow local privacy regulations (GDPR, CCPA, etc.)
- Provide clear data handling disclosures

## Security Features

### Current Implementation
- ✅ Dependency vulnerability scanning
- ✅ Secret scanning prevention  
- ✅ Automated security updates
- ✅ Input validation for file types

### Planned Implementation
- 🔄 Code signing for releases
- 🔄 Runtime application self-protection (RASP)
- 🔄 Secure configuration management
- 🔄 Audit logging for sensitive operations

## Compliance

### Privacy Regulations
- **GDPR Compliance**: User consent, data portability, right to deletion
- **CCPA Compliance**: User data rights and disclosure requirements
- **Biometric Laws**: Compliance with local biometric data regulations

### Security Standards
- Following OWASP secure coding practices
- NIST Cybersecurity Framework alignment
- Regular security assessments and updates

## Contact

For security-related questions or to report vulnerabilities:
- **GitHub Security Advisories**: Preferred method
- **Repository Issues**: For general security questions (not vulnerabilities)
- **Email**: Contact repository maintainers directly

---

**Last Updated**: September 2025  
**Version**: 1.0