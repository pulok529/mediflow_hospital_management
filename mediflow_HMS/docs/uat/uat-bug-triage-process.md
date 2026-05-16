# UAT Bug Triage Process

## Severity Levels
- Sev 1 (Blocker): workflow cannot proceed, data-loss/security risk
- Sev 2 (High): major function degraded, no easy workaround
- Sev 3 (Medium): function works with workaround, usability issue
- Sev 4 (Low): cosmetic/minor

## Triage Workflow
1. Capture issue with role, workflow step, expected vs actual, screenshot/log
2. Reproduce in staging and tag module owner
3. Assign severity + target release (hotfix/current sprint/next sprint)
4. Link fix PR and test evidence
5. Mark resolved only after tester re-validation

## Daily UAT Standup Template
- New defects count by severity
- Fixed defects awaiting retest
- Blockers requiring decision/escalation
- Go-live risk status
